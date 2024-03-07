using Fabgrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AxisConvertSystem
{
    public class ObjectUnit : PoolableMono,IProvidableFieldInfo
    {
        [HideInInspector] public CompressLayer compressLayer = CompressLayer.Default;
        [HideInInspector] public bool climbableUnit = false;
        [HideInInspector] public bool staticUnit = true;
        [HideInInspector] public bool activeUnit = true;
        
        [HideInInspector] public LayerMask canStandMask;
        [HideInInspector] public bool useGravity = true;

        public AxisConverter Converter { get; protected set; }
        public Collider Collider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public UnitDepthHandler DepthHandler { get; private set; }
        public Section Section { get; protected set; }
        public bool IsHide { get; private set; }

        protected UnitInfo OriginUnitInfo;
        private UnitInfo _unitInfo;
        private UnitInfo _convertedInfo;

        public virtual void Awake()
        {
            IsHide = false;
            
            Section = GetComponentInParent<Section>();
            Collider = GetComponent<Collider>();
            if (!staticUnit)
            {
                Rigidbody = GetComponent<Rigidbody>();
                Rigidbody.useGravity = false;
                Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            DepthHandler = new UnitDepthHandler(this);
            
            Activate(activeUnit);
        }

        public virtual void FixedUpdate()
        {
            if (!staticUnit)
            {
                if (useGravity)
                {
                    Rigidbody.AddForce(Physics.gravity * GameManager.Instance.CoreData.gravityScale, ForceMode.Acceleration);
                }
            }
        }

        public virtual void UpdateUnit()
        {
            if (!staticUnit)
            {
                if (transform.position.y <= GameManager.Instance.CoreData.destroyedDepth)
                {
                    ReloadUnit();
                }
            }
        }

        public virtual void Init(AxisConverter converter)
        {
            Converter ??= converter;

            OriginUnitInfo.LocalPos = transform.localPosition;
            OriginUnitInfo.LocalRot = transform.localRotation;
            OriginUnitInfo.LocalScale = transform.localScale;
            OriginUnitInfo.ColliderCenter = Collider.GetLocalCenter();
            _unitInfo = OriginUnitInfo;
            
            DepthHandler.DepthCheckPointSetting();
        }

        public virtual void Convert(AxisType axis)
        {
            if (!activeUnit)
            {
                return;
            }

            if (!staticUnit)
            {
                DepthHandler.DepthCheckPointSetting();
            }
            SynchronizationUnitPos(axis);
            _convertedInfo = ConvertInfo(_unitInfo, axis);
        }
        
        public virtual void UnitSetting(AxisType axis)
        {
            if (!activeUnit)
            {
                ApplyInfo(OriginUnitInfo, false);
                return;
            }
            
            ApplyInfo(_convertedInfo);

            if (IsHide)
            {
                return;
            }

            if (climbableUnit)
            {
                Collider.isTrigger = axis == AxisType.Y;
            }
            
            if (!staticUnit)
            {
                Rigidbody.FreezeAxisPosition(axis);
            }
        }

        private void ApplyInfo(UnitInfo info, bool hideSetting = true)
        {
            transform.localPosition = info.LocalPos;
            transform.localRotation = info.LocalRot;
            transform.localScale = info.LocalScale;
            Collider.SetCenter(info.ColliderCenter);
            if (hideSetting)
            {
                Hide(Math.Abs(DepthHandler.Depth - float.MaxValue) >= 0.01f);
            }
        }
        
        private UnitInfo ConvertInfo(UnitInfo basic, AxisType axis)
        {
            if (axis == AxisType.None)
            {
                return basic;
            }

            var layerDepth = (float)compressLayer * Vector3ExtensionMethod.GetAxisDir(axis).GetAxisElement(axis);
            
            basic.LocalPos.SetAxisElement(axis, layerDepth);

            basic.LocalScale = Quaternion.Inverse(basic.LocalRot) * basic.LocalScale;
            basic.LocalScale.SetAxisElement(axis, 1);
            basic.LocalScale = basic.LocalRot * basic.LocalScale;
            
            basic.ColliderCenter = basic.LocalRot * basic.ColliderCenter;
            basic.ColliderCenter.SetAxisElement(axis, -layerDepth);
            basic.ColliderCenter = Quaternion.Inverse(basic.LocalRot) * basic.ColliderCenter;

            return basic;
        }

        public void Activate(bool active)
        {
            activeUnit = active;
            gameObject.SetActive(active);
            Collider.enabled = active;
        }

        private void Hide(bool hide)
        {
            IsHide = hide;
            gameObject.SetActive(!hide);
            Collider.enabled = !hide;
        }

        public void SetPosition(Vector3 pos)
        {
            pos.SetAxisElement(Converter.AxisType, transform.position.GetAxisElement(Converter.AxisType));
            transform.position = pos;
        }

        public void SetVelocity(Vector3 velocity, bool useGravity = true)
        {
            if (staticUnit)
            {
                Debug.LogWarning("[ObjectUnit] this unit is not dynamic object.");
                return;
            }
            
            if (useGravity)
            {
                velocity.y = Rigidbody.velocity.y;
            }
            Rigidbody.velocity = velocity;
        }

        public void StopImmediately(bool withYAxis)
        {
            if (staticUnit)
            {
                Debug.LogWarning("[ObjectUnit] this unit is not dynamic object.");
                return;
            }
            
            Rigidbody.velocity = withYAxis ? Vector3.zero : new Vector3(0, Rigidbody.velocity.y, 0);
        }

        public virtual void ReloadUnit()
        {
            if (staticUnit)
            {
                Rigidbody.velocity = Vector3.zero;
            }
            
            _unitInfo = OriginUnitInfo;
            DepthHandler.CalcDepth(Converter.AxisType);
            _convertedInfo = ConvertInfo(_unitInfo, Converter.AxisType);
            UnitSetting(Converter.AxisType);
        }
        
        private void SynchronizationUnitPos(AxisType axis)
        {
            if (staticUnit || IsHide)
            {
                return;
            }
            
            if (axis == AxisType.None)
            {
                CheckStandObject();
            }
            else
            {
                _unitInfo.LocalPos = transform.localPosition;
            }
        }

        private void CheckStandObject()
        {
            var origin = Collider.bounds.center;
            if (Converter.AxisType == AxisType.Y)
            {
                ++origin.y;
            }
            
            var dir = Vector3.down;

            var isHit = Physics.Raycast(origin, dir, out var hit, Mathf.Infinity, canStandMask);
        
            if (!isHit)
            {
                return;
            }

            if (hit.transform.TryGetComponent<ObjectUnit>(out var unit))
            {
                var info = unit._unitInfo;
                
                var diff = hit.point - hit.collider.bounds.center;
                diff.y = 0;
                
                var standPos = hit.transform.localPosition + diff;
                standPos.SetAxisElement(Converter.AxisType, info.LocalPos.GetAxisElement(Converter.AxisType));
                standPos.y += info.LocalScale.y / 2f;
                
                _unitInfo.LocalPos = standPos;
            }
            else
            {
                var diff = hit.point - hit.collider.bounds.center;
                var standPos = hit.transform.localPosition + diff;
                standPos.SetAxisElement(Converter.AxisType, _unitInfo.LocalPos.GetAxisElement(Converter.AxisType));
                _unitInfo.LocalPos = standPos;
            }
        }

        public override void OnPop()
        {
        }

        public override void OnPush()
        {
        }

        public List<FieldInfo> GetFieldInfos()
        {
            Type type = this.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return fields.ToList();
        }

        public void SetFieldInfos(List<FieldInfo> infos)
        {
            if (infos == null)
            {
                Debug.Log("Info is null");
                return;
            }

            foreach (FieldInfo info in infos)
            {
                try
                {
                    object value = FieldInfoStorage.GetFieldValue(info.FieldType);
                    info.SetValue(this, value);
                }
                catch
                {
                    Debug.Log($"This info can't set value: {info}");
                }
            }
        }
    }
}