using Fabgrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

namespace AxisConvertSystem
{
    public class ObjectUnit : PoolableMono, IProvidableFieldInfo
    {
        [HideInInspector] public CompressLayer compressLayer = CompressLayer.Platform;
        [HideInInspector] public UnitRenderType renderType = UnitRenderType.Opaque;
        [HideInInspector] public bool climbableUnit = false;
        [HideInInspector] public bool staticUnit = true;
        [HideInInspector] public bool activeUnit = true;
        [HideInInspector] public bool subUnit = false;
        
        [HideInInspector] public LayerMask canStandMask;
        [HideInInspector] public float checkOffset = 0.2f; 
        [HideInInspector] public bool useGravity = true;

        protected UnitInfo OriginUnitInfo;
        private UnitInfo _unitInfo;
        protected UnitInfo ConvertedInfo;

        public AxisConverter Converter { get; protected set; }
        public Collider Collider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public UnitDepthHandler DepthHandler { get; private set; }
        public Section Section { get; protected set; }
        public bool IsHide { get; private set; }
        public bool OnGround => CheckStandObject(out var tmp, true);

        private List<Renderer> _renderers;
        private List<Material> _materials;

        private LayerMask _climbLayerMask;
        private UnClimbableEffect _unClimbableEffect;

        private readonly int _dissolveProgressHash = Shader.PropertyToID("_DissolveProgress");
        private readonly int _visibleProgressHash = Shader.PropertyToID("_VisibleProgress");
        
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
            DepthHandler = new UnitDepthHandler(this);

            _materials = new List<Material>();
            _renderers = new List<Renderer>();
            transform.GetComponentsInChildren<Renderer>(_renderers);
            MaterialResetUp();
            
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
                ConvertedInfo = OriginUnitInfo;
                return;
            }

            if (!staticUnit)
            {
                DepthHandler.DepthCheckPointSetting();
            }
            SynchronizePosition(axis);
            ConvertedInfo = ConvertInfo(_unitInfo, axis);
        }
        
        public virtual void UnitSetting(AxisType axis)
        {
            ApplyInfo(ConvertedInfo);

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
        }

        public virtual void DepthSetting()
        {
            if (!activeUnit)
            {
                return;
            }

            Hide(DepthHandler.Hide);
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

            var layerDepth = (int)compressLayer * Vector3ExtensionMethod.GetAxisDir(axis).GetAxisElement(axis);

            if (!subUnit)
            {
                basic.LocalPos.SetAxisElement(axis, layerDepth);
            }

            basic.LocalScale = Quaternion.Inverse(basic.LocalRot) * basic.LocalScale;
            basic.LocalScale.SetAxisElement(axis, 1);
            basic.LocalScale = basic.LocalRot * basic.LocalScale;

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
                UnitSetting(Converter.AxisType);
                DepthSetting();
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

        public virtual void ReloadUnit(float dissolveTime = 2f, Action callBack = null)
        {
            _unitInfo = OriginUnitInfo;
            DepthHandler.CalcDepth(Converter.AxisType);
            ConvertedInfo = ConvertInfo(_unitInfo, Converter.AxisType);
            UnitSetting(Converter.AxisType);
            Physics.SyncTransforms();

            if (!staticUnit)
            {
                Dissolve(0f, dissolveTime, true, callBack);
                Rigidbody.velocity = Vector3.zero;
                PlaySpawnVFX();
            }
        }
        
        public void RewriteUnitInfo()
        {
            _unitInfo.LocalPos = transform.localPosition;
            _unitInfo.LocalRot = transform.localRotation;
            _unitInfo.LocalScale = transform.localScale;
            _unitInfo.ColliderCenter = Collider.GetLocalCenter();
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
                        _unitInfo.LocalPos = transform.localPosition;
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
            var unit = col.transform.GetComponent<ObjectUnit>();
            var info = unit._unitInfo;

            var standPos = transform.localPosition;
            standPos.y = col.bounds.max.y;
            
            var standUnitLocalPos = info.LocalPos;
            if (unit.subUnit)
            {
                var parentUnit = unit.GetParentUnit();
                info = parentUnit._unitInfo;
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
                    (unit is PlaneUnit or TutorialObjectUnit ? _unitInfo.LocalPos : standUnitLocalPos)
                    .GetAxisElement(Converter.AxisType));
            }

            _unitInfo.LocalPos = standPos;
        }

        public bool CheckStandObject(out Collider col, bool ignoreTriggered = false)
        {
            var origin = Collider.bounds.center;
            var triggerInteraction = ignoreTriggered ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide;
            
            if (Converter.AxisType == AxisType.Y)
            {
                var cols = new Collider[10];
                Physics.OverlapBoxNonAlloc(origin, Vector3.one * 0.1f, cols, Quaternion.identity, canStandMask, triggerInteraction);
                col = cols[0];

                if (col is null)
                {
                    Physics.OverlapBoxNonAlloc(origin - Vector3.up, Vector3.one * 0.1f, cols, Quaternion.identity, canStandMask, triggerInteraction);
                    col = cols[0];
                }
                
                return col;
            }
            else
            {
                var dir = Vector3.down;
                var distance = Collider.bounds.size.y / 2f + checkOffset;
                
                var isHit = Physics.Raycast(origin, dir, out var hit, distance, canStandMask, triggerInteraction);
                col = isHit ? hit.collider : null;
                return isHit;
            }
        }
        
        public void Dissolve(float value, float time, bool useDissolve = true, Action callBack = null)
        {
            value = Mathf.Clamp(value, 0f, 1f);
        
            foreach (var material in _materials)
            {
                var initVal = Mathf.Abs(1f - value);
                if (useDissolve)
                {
                    material.SetFloat(_dissolveProgressHash, initVal);
                }
                material.SetFloat(_visibleProgressHash, initVal);
            }

            var seq = DOTween.Sequence();

            foreach (var material in _materials)
            {
                if (useDissolve)
                {
                    seq.Join(DOTween.To(() => material.GetFloat(_dissolveProgressHash),
                        progress => material.SetFloat(_dissolveProgressHash, progress), value, time));
                }
                seq.Join(DOTween.To(() => material.GetFloat(_visibleProgressHash),
                    progress => material.SetFloat(_visibleProgressHash, progress), value, time));
            }

            seq.OnComplete(() => callBack?.Invoke());
        }

        private void PlaySpawnVFX()
        {
            var spawnVFX = PoolManager.Instance.Pop("SpawnVFX") as PoolableVFX;
            var bounds = Collider.bounds;
            var position = transform.position;
            position.y = bounds.min.y;

            spawnVFX.SetPositionAndRotation(position, Quaternion.identity);
            spawnVFX.SetScale(new Vector3(bounds.size.x, 1, bounds.size.z));
            spawnVFX.Play();
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

        public void ShowUnClimbableEffect()
        {
            if (CanAppearClimbable())
            {
                if(_unClimbableEffect == null)
                {
                    _unClimbableEffect = SceneControlManager.Instance.AddObject("UnClimbableEffect") as UnClimbableEffect;
                    _unClimbableEffect.Setting(Collider);
                }
            }
        }

        public void UnShowClimbableEffect()
        {
            if(_unClimbableEffect != null)
            {
                SceneControlManager.Instance.DeleteObject(_unClimbableEffect);
                _unClimbableEffect = null;
            }
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
    }
}