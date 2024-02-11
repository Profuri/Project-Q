using System;
using System.Collections.Generic;
using UnityEngine;

namespace AxisConvertSystem
{
    public class ObjectUnit : PoolableMono
    {
        [HideInInspector] public CompressLayer compressLayer = CompressLayer.Default;
        [HideInInspector] public bool staticUnit = true;
        
        [HideInInspector] public LayerMask canStandMask;
        [HideInInspector] public float rayDistance;

        public AxisConverter Converter { get; protected set; }
        public Collider Collider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        private UnitInfo _originUnitInfo;
        private UnitInfo _basicUnitInfo;
        
        private Dictionary<AxisType, List<Vector3>> _depthCheckPoint;

        private float _depth;

        public virtual void Awake()
        {
            Collider = GetComponent<Collider>();
            if (!staticUnit)
            {
                Rigidbody = GetComponent<Rigidbody>();
            }
            
            _depthCheckPoint = new Dictionary<AxisType, List<Vector3>>();
        }

        public virtual void Init(AxisConverter converter)
        {
            Converter ??= converter;

            _originUnitInfo.LocalPos = transform.localPosition;
            _originUnitInfo.LocalRot = transform.localRotation;
            _originUnitInfo.LocalScale = transform.localScale;
            _originUnitInfo.ColliderCenter = Collider.bounds.center - transform.position;
            _basicUnitInfo = _originUnitInfo;
            
            CalcDepthCheckPoint();
        }

        public void CalcDepth(AxisType axis)
        {
            if (axis == AxisType.None)
            {
                _depth = float.MaxValue;
                return;
            }
            
            if (!staticUnit)
            {
                CalcDepthCheckPoint();
            }
            _depth = 0f;
            
            for (var i = 0; i < 4; i++)
            {
                var isHit = Physics.Raycast(_depthCheckPoint[axis][i], Vector3ExtensionMethod.GetAxisDir(axis),
                    out var hit, Mathf.Infinity, Converter.ObjectMask);
                var temp = isHit ? Mathf.Abs(hit.point.GetAxisElement(axis)) : float.MaxValue;
                
                _depth = Mathf.Max(_depth, temp);
            }
        }
        
        public void SynchronizationUnitPos(AxisType axis)
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
                _basicUnitInfo.LocalPos = transform.localPosition;
            }
        }

        public void Convert(AxisType axis)
        {
            var unitInfo = ConvertInfo(_basicUnitInfo, axis);
            UnitSetting(axis, unitInfo);
            Activate(Math.Abs(_depth - float.MaxValue) < 0.01f);
        }

        protected void UnitSetting(AxisType axis, UnitInfo unitInfo)
        {
            transform.localPosition = unitInfo.LocalPos;
            transform.localRotation = unitInfo.LocalRot;
            transform.localScale = unitInfo.LocalScale;
            Collider.SetCenter(unitInfo.ColliderCenter);

            if (!staticUnit)
            {
                Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                Rigidbody.FreezeAxisPosition(axis);
            }
        }

        protected UnitInfo ConvertInfo(UnitInfo basic, AxisType axis)
        {
            if (axis == AxisType.None)
            {
                return basic;
            }

            var colSize = Collider.bounds.size.GetAxisElement(axis) - basic.LocalScale.GetAxisElement(axis);
            var layerDepth = ((float)compressLayer + ((int)colSize + (colSize - (int)colSize) / 2f))
                * Vector3ExtensionMethod.GetAxisDir(axis).GetAxisElement(axis);
            var colliderCenter = -Mathf.Abs(layerDepth - (Collider.bounds.center - transform.position).GetAxisElement(axis));

            basic.LocalPos.SetAxisElement(axis, layerDepth);
            basic.LocalScale.SetAxisElement(axis, 1);
            basic.ColliderCenter.SetAxisElement(axis, colliderCenter);

            return basic;
        }

        protected void CalcDepthCheckPoint()
        {
            _depthCheckPoint.Clear();
            _depthCheckPoint.Add(AxisType.X, Vector3ExtensionMethod.CalcAxisBounds(AxisType.X, Collider.bounds));
            _depthCheckPoint.Add(AxisType.Y, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Y, Collider.bounds));
            _depthCheckPoint.Add(AxisType.Z, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Z, Collider.bounds));
        }

        protected void Activate(bool active)
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
            var info = hit.transform.TryGetComponent<ObjectUnit>(out var unit) ? unit._basicUnitInfo : _basicUnitInfo;
            standPos.SetAxisElement(Converter.AxisType, info.LocalPos.GetAxisElement(Converter.AxisType));
            _basicUnitInfo.LocalPos = standPos;
        }

        public override void OnPop()
        {
        }

        public override void OnPush()
        {
        }
    }
}