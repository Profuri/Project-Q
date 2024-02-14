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
            }
            DepthHandler = new UnitDepthHandler(this);
        }

        public virtual void Init(AxisConverter converter)
        {
            Converter ??= converter;

            OriginUnitInfo.LocalPos = transform.localPosition;
            OriginUnitInfo.LocalRot = transform.localRotation;
            OriginUnitInfo.LocalScale = transform.localScale;
            OriginUnitInfo.ColliderCenter = Collider.bounds.center - transform.position;
            UnitInfo = OriginUnitInfo;
            
            DepthHandler.DepthCheckPointSetting();
        }

        public void Convert(AxisType axis)
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

        public void UnitSetting(AxisType axis)
        {
            transform.localPosition = ConvertedInfo.LocalPos;
            transform.localRotation = ConvertedInfo.LocalRot;
            transform.localScale = ConvertedInfo.LocalScale;
            Collider.SetCenter(ConvertedInfo.ColliderCenter);
            Activate(Math.Abs(DepthHandler.Depth - float.MaxValue) < 0.01f);

            if (!staticUnit)
            {
                Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                Rigidbody.FreezeAxisPosition(axis);
            }
        }

        public void ReloadUnit()
        {
            UnitInfo = OriginUnitInfo;
            
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

            var colSize = Collider.bounds.size.GetAxisElement(axis) - basic.LocalScale.GetAxisElement(axis);
            var layerDepth = ((float)compressLayer + ((int)colSize + (colSize - (int)colSize) / 2f))
                * Vector3ExtensionMethod.GetAxisDir(axis).GetAxisElement(axis);
            var colliderCenter = -layerDepth + (Collider.bounds.center - transform.position).GetAxisElement(axis);

            basic.LocalPos.SetAxisElement(axis, layerDepth);
            basic.LocalScale.SetAxisElement(axis, 1);
            basic.ColliderCenter.SetAxisElement(axis, colliderCenter);

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
    }
}