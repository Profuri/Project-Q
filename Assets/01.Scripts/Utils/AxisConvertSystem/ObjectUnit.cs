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
        [HideInInspector] public bool staticUnit = true;
        
        [HideInInspector] public LayerMask canStandMask;
        [HideInInspector] public float rayDistance;

        public AxisConverter Converter { get; protected set; }
        public Collider Collider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public UnitDepthHandler DepthHandler { get; private set; }

        public UnitInfo OriginUnitInfo;
        public UnitInfo UnitInfo;
        public UnitInfo ConvertedInfo;

        public virtual void Awake()
        {
            Collider = GetComponent<Collider>();
            if (!staticUnit)
            {
                Rigidbody = GetComponent<Rigidbody>();
                Rigidbody.useGravity = false;
                Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            DepthHandler = new UnitDepthHandler(this);
        }

        public virtual void Update()
        {
            if (!staticUnit)
            {
                Rigidbody.AddForce(Vector3.up * GameManager.Instance.CoreData.gravity);

                if (transform.position.y <= GameManager.Instance.CoreData.destroyedDepth)
                {
                    ReloadUnit();
                    return;
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
            UnitInfo = OriginUnitInfo;
            
            DepthHandler.DepthCheckPointSetting();
        }

        public virtual void Convert(AxisType axis)
        {
            DepthHandler.CalcDepth(axis);
            SynchronizationUnitPos(axis);
            ConvertedInfo = ConvertInfo(UnitInfo, axis);
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

        public virtual void UnitSetting(AxisType axis)
        {
            transform.localPosition = ConvertedInfo.LocalPos;
            transform.localRotation = ConvertedInfo.LocalRot;
            transform.localScale = ConvertedInfo.LocalScale;
            Collider.SetCenter(ConvertedInfo.ColliderCenter);
            Activate(Math.Abs(DepthHandler.Depth - float.MaxValue) < 0.01f);

            if (!staticUnit)
            {
                Rigidbody.FreezeAxisPosition(axis);
            }
        }

        public void ReloadUnit()
        {
            if (staticUnit)
            {
                Rigidbody.velocity = Vector3.zero;
            }
            
            UnitInfo = OriginUnitInfo;
            DepthHandler.CalcDepth(Converter.AxisType);
            ConvertedInfo = ConvertInfo(UnitInfo, Converter.AxisType);
            UnitSetting(Converter.AxisType);
        }
        
        private void SynchronizationUnitPos(AxisType axis)
        {
            if (staticUnit)
            {
                return;
            }
            
            if (axis == AxisType.None)
            {
                CheckStandObject();
            }
            else
            {
                UnitInfo.LocalPos = transform.localPosition;
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

        private void Activate(bool active)
        {
            gameObject.SetActive(active);
            Collider.enabled = active;
        }

        private void CheckStandObject()
        {
            var origin = Collider.bounds.center;
            var dir = Vector3.down;

            var isHit = Physics.Raycast(origin, dir, out var hit, rayDistance, canStandMask);
        
            if (!isHit)
            {
                return;
            }

            var diff = hit.point - hit.collider.bounds.center;
            var standPos = hit.transform.localPosition + diff;
            var info = hit.transform.TryGetComponent<ObjectUnit>(out var unit) ? unit.UnitInfo : UnitInfo;
            standPos.SetAxisElement(Converter.AxisType, info.LocalPos.GetAxisElement(Converter.AxisType));
            UnitInfo.LocalPos = standPos;
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