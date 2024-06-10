using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AxisConvertSystem
{
    public class ObjectUnit : PoolableMono
    {
        [HideInInspector] public CompressLayer compressLayer = CompressLayer.Platform;
        [HideInInspector] public float offset = 0;
        [HideInInspector] public UnitRenderType renderType = UnitRenderType.Opaque;
        [HideInInspector] public bool climbableUnit = false;
        [HideInInspector] public bool staticUnit = true;
        [HideInInspector] public bool activeUnit = true;
        [HideInInspector] public bool subUnit = false;
        
        [HideInInspector] public LayerMask canStandMask;
        [HideInInspector] public float checkOffset = 0.2f; 
        [HideInInspector] public bool useGravity = true;

        public UnitInfo OriginUnitInfo;
        public UnitInfo UnitInfo;
        public UnitInfo ConvertedInfo;

        public AxisConverter Converter { get; protected set; }
        public Collider Collider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public UnitDepthHandler DepthHandler { get; private set; }
        public Section Section { get; protected set; }
        public bool IsHide { get; private set; }
        public bool OnGround => CheckStandObject(out var tempCollider, true);
        
        public List<ObjectUnit> IntersectedUnits { get; private set; }

        private List<Renderer> _renderers;
        private List<Material> _materials;

        private LayerMask _climbLayerMask;
        private UnClimbableEffect _unClimbableEffect;

        private readonly int _dissolveProgressHash = Shader.PropertyToID("_DissolveProgress");
        private readonly int _visibleProgressHash = Shader.PropertyToID("_VisibleProgress");
        
        // Events
        public event Action<AxisConverter> OnInitEvent;
        public event Action<AxisType> OnConvertEvent;
        public event Action<AxisType> OnApplyUnitInfoEvent;
        
        public override void OnPop()
        {
            
        }

        public override void OnPush()
        {

        }

        public virtual void Awake()
        {
            IsHide = false;

            _climbLayerMask = LayerMask.GetMask("Player") | LayerMask.GetMask("Platform");

            Section = GetComponentInParent<Section>();
            
            Collider = GetComponent<Collider>();
            if (!staticUnit)
            {
                Rigidbody = GetComponent<Rigidbody>();
                Rigidbody.useGravity = false;
                Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            IntersectedUnits = new List<ObjectUnit>();
            DepthHandler = new UnitDepthHandler(this);
            
            _materials = new List<Material>();
            _renderers = new List<Renderer>();
            transform.GetComponentsInChildren(_renderers);
            MaterialResetUp();
            
            OriginUnitInfo.LocalPos = transform.localPosition;
            OriginUnitInfo.LocalRot = transform.localRotation;
            OriginUnitInfo.LocalScale = transform.localScale;
            OriginUnitInfo.ColliderCenter = Collider.GetLocalCenter();
            OriginUnitInfo.ColliderBoundSize = Collider.bounds.size;
            
            UnitInfo = OriginUnitInfo;

            Activate(activeUnit);
        }

        public virtual void FixedUpdateUnit()
        {
            if (staticUnit || !useGravity)
            {
                return;
            }

            if (Converter.AxisType == AxisType.Y && OnGround)
            {
                return;
            }
            
            Rigidbody.AddForce(Physics.gravity * GameManager.Instance.CoreData.gravityScale, ForceMode.Acceleration);
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
            
            DepthHandler.DepthCheckPointSetting();
            
            OnInitEvent?.Invoke(converter);

            if(CanAppearClimbable() && Section is Stage)
            {
                DeleteClimbableEffect();
                _unClimbableEffect = PoolManager.Instance.Pop("UnClimbableEffect") as UnClimbableEffect;
                _unClimbableEffect?.Setting(this);
            }
        }

        public void DeleteClimbableEffect()
        {
            if (_unClimbableEffect != null)
            {
                PoolManager.Instance.Push(_unClimbableEffect);
            }
        }

        public virtual void Convert(AxisType axis)
        {
            if (!activeUnit)
            {
                ConvertedInfo = OriginUnitInfo;
                return;
            }

            if (!staticUnit && axis != AxisType.None)
            {
                DepthHandler.DepthCheckPointSetting();
            }
            
            SynchronizePosition(axis);
            ConvertedInfo = ConvertInfo(UnitInfo, axis);

            OnConvertEvent?.Invoke(axis);
        }
        
        public virtual void ApplyUnitInfo(AxisType axis)
        {
            if (!staticUnit && axis == AxisType.None && Converter.AxisType != AxisType.None)
            {
                var localPos = transform.localPosition;
                localPos.SetAxisElement(Converter.AxisType, UnitInfo.LocalPos.GetAxisElement(Converter.AxisType));
                ConvertedInfo.LocalPos = localPos;
            }
            
            ApplyInfo(ConvertedInfo);

            if (CanAppearClimbable() && _unClimbableEffect != null)
            {
                _unClimbableEffect.SetAlpha(0f);
            }

            if (DepthHandler.Hide)
            {
                return;
            }

            if (climbableUnit)
            {
                Collider.excludeLayers = axis == AxisType.Y ? _climbLayerMask : 0;
            }
            
            if (!staticUnit && axis != AxisType.Y)
            {
                Rigidbody.FreezeAxisPosition(axis);
            }
            
            OnApplyUnitInfoEvent?.Invoke(axis);
        }


        public virtual void ApplyDepth()
        {
            if (!activeUnit)
            {
                return;
            }

            Hide(DepthHandler.Hide);
        }

        public virtual void OnCameraSetting(AxisType axis)
        {
                        
        }

        private void ApplyInfo(UnitInfo info)
        {
            transform.localPosition = info.LocalPos;
            transform.localRotation = info.LocalRot;
            transform.localScale = info.LocalScale;
            Collider.SetCenter(info.ColliderCenter);
        }
        
        protected virtual UnitInfo ConvertInfo(UnitInfo basic, AxisType axis)
        {
            if (axis == AxisType.None)
            {
                return basic;
            }

            var layerDepth = ((int)compressLayer + offset) * Vector3ExtensionMethod.GetAxisDir(axis).GetAxisElement(axis);

            if (!subUnit)
            {
                basic.LocalPos.SetAxisElement(axis, layerDepth);
            }

            basic.LocalScale = basic.LocalRot * basic.LocalScale;
            basic.LocalScale.SetAxisElement(axis, 1);
            basic.LocalScale = (Quaternion.Inverse(basic.LocalRot) * basic.LocalScale).Abs();

            basic.ColliderCenter = basic.LocalRot * basic.ColliderCenter;
            basic.ColliderCenter.SetAxisElement(axis, -layerDepth);
            basic.ColliderCenter = Quaternion.Inverse(basic.LocalRot) * basic.ColliderCenter;

            return basic;
        }

        public virtual void Activate(bool active)
        {
            if (activeUnit == active)
            {
                return;
            }

            activeUnit = active;
            Collider.enabled = active;

            if (activeUnit)
            {
                gameObject.SetActive(true);
                Dissolve(0f, 0.5f);
                
                Convert(Converter.AxisType);
                ApplyUnitInfo(Converter.AxisType);
                ApplyDepth();
            }
            else
            {
                Dissolve(active ? 0f : 1f, 0.5f, true, () => 
                {
                    gameObject.SetActive(false);    
                });
            }
        }

        protected virtual void Hide(bool hide)
        {
            IsHide = hide;
            Collider.enabled = !hide;
            gameObject.SetActive(!hide);
        }

        public void SetPosition(Vector3 pos)
        {
            pos.SetAxisElement(Converter.AxisType, transform.position.GetAxisElement(Converter.AxisType));
            transform.position = pos;
        }

        public void SetVelocity(Vector3 velocity, bool useGravity = true)
        {
            if (staticUnit || Rigidbody.isKinematic)
            {
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
            if (staticUnit || Rigidbody.isKinematic)
            {
                return;
            }
            
            Rigidbody.velocity = withYAxis ? Vector3.zero : new Vector3(0, Rigidbody.velocity.y, 0);
        }

        public virtual void ReloadUnit(bool useDissolve = false, float dissolveTime = 2f, Action callBack = null)
        {
            Debug.Log(OriginUnitInfo.LocalPos);
            UnitInfo = OriginUnitInfo;
            DepthHandler.CalcDepth(Converter.AxisType);
            ConvertedInfo = ConvertInfo(UnitInfo, Converter.AxisType);
            ApplyUnitInfo(Converter.AxisType);
            Physics.SyncTransforms();

            if (!staticUnit)
            {
                Rigidbody.velocity = Vector3.zero;
                Dissolve(0f, useDissolve ? dissolveTime : 0f, true, callBack);
            }
        }
        
        public void RewriteUnitInfo()
        {
            UnitInfo.LocalPos = transform.localPosition;
            UnitInfo.LocalRot = transform.localRotation;
            UnitInfo.LocalScale = transform.localScale;
            UnitInfo.ColliderCenter = Collider.GetLocalCenter();
            UnitInfo.ColliderBoundSize = Collider.bounds.size;
        }

        private void SynchronizePosition(AxisType axis)
        {
            if (staticUnit || IsHide)
            {
                return;
            }

            if (axis == AxisType.None)
            {
                if (CheckStandObject(out var col))
                {
                    if (!subUnit)
                    {
                        SynchronizePositionOnStanding(col);
                    }
                    else
                    {
                        UnitInfo.LocalPos = transform.localPosition;
                    }
                }
            }
            else
            {
                RewriteUnitInfo();
            }
        }

        private void SynchronizePositionOnStanding(Collider col)
        {
            var standUnit = col.transform.GetComponent<ObjectUnit>();
            var unit = FlippingStandUnit(standUnit);
            
            var info = unit.UnitInfo;

            var standPos = transform.localPosition;
            standPos.y = col.bounds.max.y;
            
            var standUnitLocalPos = info.LocalPos;
            if (unit.subUnit)
            {
                var parentUnit = unit.GetParentUnit();
                info = parentUnit.UnitInfo;
                standUnitLocalPos += info.LocalPos;
            }
            
            if (Converter.AxisType == AxisType.Y)
            {
                standPos.y *= info.LocalScale.y;
                standPos.SetAxisElement(Converter.AxisType, standPos.y + standUnitLocalPos.GetAxisElement(Converter.AxisType));
            }
            else
            {
                standPos.SetAxisElement(Converter.AxisType,
                    (unit is PlaneUnit or TutorialObjectUnit ? UnitInfo.LocalPos : standUnitLocalPos)
                    .GetAxisElement(Converter.AxisType));
            }

            UnitInfo.LocalPos = standPos;
        }

        private ObjectUnit FlippingStandUnit(ObjectUnit standUnit)
        {
            if (standUnit.IntersectedUnits.Count <= 0 || Converter.AxisType == AxisType.Y)
            {
                return standUnit;
            }
        
            var axis = Converter.AxisType;
            var depth = DepthHandler.GetDepth(Converter.AxisType);

            var frontDepth = standUnit.DepthHandler.GetDepth(Converter.AxisType);
            var boundsSize = (standUnit.UnitInfo.LocalRot * standUnit.UnitInfo.ColliderBoundSize).GetAxisElement(axis);

            // is in back unit
            if (depth > frontDepth || depth < frontDepth - boundsSize)
            {
                return standUnit;
            }

            // is in front unit
            var backUnit = standUnit;
            var dynamicDepthPoint = Collider.GetDepthPoint(Converter.AxisType);
            var dynamicVirtualDepth = DepthHandler.GetDepth(AxisType.Y) - Collider.bounds.size.y; 
            
            // find back unit
            foreach (var intersectedUnit in standUnit.IntersectedUnits)
            {
                if (intersectedUnit == this)
                {
                    continue;
                }

                var intersectedUnitDepthPoint = intersectedUnit.DepthHandler.GetDepthPoint(Converter.AxisType);
                if ((dynamicDepthPoint.Min.x < intersectedUnitDepthPoint.Min.x && dynamicDepthPoint.Max.x < intersectedUnitDepthPoint.Min.x) ||
                    (dynamicDepthPoint.Min.x > intersectedUnitDepthPoint.Max.x && dynamicDepthPoint.Max.x > intersectedUnitDepthPoint.Max.x))
                {
                    continue;
                }

                var intersectedUnitVerticalDepth = intersectedUnit.DepthHandler.GetDepth(AxisType.Y);
                if (Mathf.Abs(intersectedUnitVerticalDepth - dynamicVirtualDepth) > checkOffset)
                {
                    continue;
                }

                var currentUnitDepth = backUnit.DepthHandler.GetDepth(Converter.AxisType);
                var intersectedUnitDepth = intersectedUnit.DepthHandler.GetDepth(Converter.AxisType);

                if (currentUnitDepth >= intersectedUnitDepth)
                {
                    backUnit = intersectedUnit;
                }
            }

            return backUnit;
        }

        public bool CheckStandObject(out Collider col, bool ignoreTriggered = false)
        {
            var origin = Collider.bounds.center;
            var triggerInteraction = ignoreTriggered ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide;
            
            var size = 0;
            var cols = new Collider[10];

            col = null;

            if (Converter.AxisType == AxisType.Y)
            {
                size = Physics.OverlapBoxNonAlloc(origin, Vector3.one * 0.1f, cols, Quaternion.identity, canStandMask, triggerInteraction);

                if (size <= 0)
                {
                    size = Physics.OverlapBoxNonAlloc(origin - Vector3.up, Vector3.one * 0.1f, cols, Quaternion.identity, canStandMask, triggerInteraction);
                }
            }
            else
            {
                var dir = Vector3.down;
                var distance = Collider.bounds.size.y / 2f + checkOffset;

                var results = new RaycastHit[10];
                size = Physics.RaycastNonAlloc(origin, dir, results, distance, canStandMask, triggerInteraction);

                for (var i = 0; i < size; i++)
                {
                    cols[i] = results[i].collider;
                }
            }

            for (var i = 0; i < size; i++)
            {
                if (cols[i] is null)
                {
                    continue;
                }

                if (col is null)
                {
                    col = cols[i];
                }
                else
                {
                    if (!col.TryGetComponent<ObjectUnit>(out var unit) ||
                        !cols[i].TryGetComponent<ObjectUnit>(out var otherUnit))
                    {
                        continue;
                    }
                    
                    if (unit.DepthHandler.GetDepth(Converter.AxisType) <= otherUnit.DepthHandler.GetDepth(Converter.AxisType))
                    {
                        col = cols[i];
                    }
                }
            }

            return size > 0;
        }
        
        public virtual void Dissolve(float value, float time, bool useDissolve = true, Action callBack = null)
        {
            StartSafeCoroutine("DissolveRoutine",DissolveRoutine(value,time,useDissolve,callBack));
        }

        private IEnumerator DissolveRoutine(float value, float time, bool useDissolve = true, Action callBack = null)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            var initVal = Mathf.Abs(1f - value);

            foreach (var material in _materials)
            {
                if (useDissolve)
                {
                    material.SetFloat(_dissolveProgressHash, initVal);
                }
                material.SetFloat(_visibleProgressHash, initVal);
            }

            var currentTime = 0f;

            while(currentTime <= time )
            {
                currentTime += Time.deltaTime;
                var percent = currentTime / time;
                var currentProgress = Mathf.Lerp(initVal,value,percent);

                foreach (var material in _materials)
                {
                    if (useDissolve)
                    {
                        material.SetFloat(_dissolveProgressHash, currentProgress);
                    }
                    material.SetFloat(_visibleProgressHash, currentProgress);
                }
                yield return null;
            }
            callBack?.Invoke();
        }
        public bool IsSuperiorUnit(ObjectUnit checkUnit)
        {
            if (!subUnit)
            {
                return false;
            }
            
            var checkTrm = transform;
            
            while (checkTrm.parent != null)
            {
                var parent = checkTrm.parent;
                var unit = parent.GetComponent<ObjectUnit>();

                if (unit == checkUnit)
                {
                    return true;
                }

                if (unit != null && !unit.subUnit)
                {
                    break;
                }
                
                checkTrm = parent;
            }

            return false;
        }

        public bool IsChildUnit(ObjectUnit checkUnit)
        {
            if (subUnit)
            {
                return false;
            }

            var children = transform.GetComponentsInChildren<ObjectUnit>().ToList();
            return children.Contains(checkUnit);
        }

        public ObjectUnit GetParentUnit()
        {
            if (!subUnit)
            {
                return null;
            }

            var checkTrm = transform;
            ObjectUnit parentUnit = null;
            
            while (checkTrm.parent != null)
            {
                var parent = checkTrm.parent;
                var unit = parent.GetComponent<ObjectUnit>();

                if (unit != null && !unit.subUnit)
                {
                    parentUnit = unit;
                    break;
                }
                
                checkTrm = parent;
            }

            return parentUnit;
        }

        private bool CanAppearClimbable()
        {
            bool onLayer = (int)compressLayer < (int)CompressLayer.Obstacle;
            return !climbableUnit && onLayer && !Collider.isTrigger;
        }

        protected void MaterialResetUp()
        {
            _materials.Clear();
            foreach (var rdr in _renderers)
            {
                if(!rdr.enabled)
                {
                    continue;
                }

                foreach (var material in rdr.materials) 
                {
                    _materials.Add(material);    
                }
            }
        }

        public virtual void SetGravity(bool useGravityParam)
        {
            useGravity = useGravityParam;
        }
    }
}