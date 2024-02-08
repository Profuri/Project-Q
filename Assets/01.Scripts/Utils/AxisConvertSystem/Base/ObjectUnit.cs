using System;
using System.Collections.Generic;
using UnityEngine;

namespace AxisConvertSystem
{
    public class ObjectUnit : MonoBehaviour
    {
        public CompressLayer compressLayer = CompressLayer.Default;
        public bool staticUnit = true;
        
        [HideInInspector] public LayerMask canStandMask;
        [HideInInspector] public float rayDistance;

        protected AxisConverter converter;
        protected Collider collider;
        protected Rigidbody rigid;

        protected UnitInfo originUnitInfo;
        protected UnitInfo basicUnitInfo;
        
        protected Dictionary<AxisType, List<Vector3>> depthCheckPoint;

        protected float depth;

        public virtual void Init(AxisConverter converter)
        {
            this.converter = converter;
            collider = GetComponent<Collider>();
            if (!staticUnit)
            {
                rigid = GetComponent<Rigidbody>();
            }

            originUnitInfo = new UnitInfo
            {
                LocalPos = transform.localPosition,
                LocalRot = transform.localRotation,
                LocalScale = transform.localScale,
                ColliderCenter = collider.bounds.center - transform.position
            };
            basicUnitInfo = originUnitInfo;
            
            depthCheckPoint ??= new Dictionary<AxisType, List<Vector3>>();

            CalcDepthCheckPoint();
        }

        public void CalcDepth(AxisType axis)
        {
            if (axis == AxisType.None)
            {
                depth = float.MaxValue;
                return;
            }
            
            if (!staticUnit)
            {
                CalcDepthCheckPoint();
            }
            depth = 0f;
            
            for (var i = 0; i < 4; i++)
            {
                var isHit = Physics.Raycast(depthCheckPoint[axis][i], Vector3ExtensionMethod.GetAxisDir(axis),
                    out var hit, Mathf.Infinity, converter.ObjectMask);
                var temp = isHit ? Mathf.Abs(hit.point.GetAxisElement(axis)) : float.MaxValue;
                
                depth = Mathf.Max(depth, temp);
            }
        }

        public void Convert(AxisType axis)
        {
            if (!staticUnit)
            {
                SynchronizationUnitPos(axis);
            }

            var unitInfo = ConvertInfo(basicUnitInfo, axis);
            UnitSetting(axis, unitInfo);   

            Activate(Math.Abs(depth - float.MaxValue) < 0.01f);
        }

        protected void UnitSetting(AxisType axis, UnitInfo unitInfo)
        {
            transform.localPosition = unitInfo.LocalPos;
            transform.localRotation = unitInfo.LocalRot;
            transform.localScale = unitInfo.LocalScale;
            collider.SetCenter(unitInfo.ColliderCenter);
        }

        protected UnitInfo ConvertInfo(UnitInfo basic, AxisType axis)
        {
            if (axis == AxisType.None)
            {
                return basic;
            }

            basic.LocalPos.SetAxisElement(axis, (int)compressLayer * Vector3ExtensionMethod.GetAxisDir(axis).GetAxisElement(axis));
            basic.LocalScale.SetAxisElement(axis, 1);
            basic.ColliderCenter.SetAxisElement(axis, -(int)compressLayer * Vector3ExtensionMethod.GetAxisDir(axis).GetAxisElement(axis));

            return basic;
        }

        protected void CalcDepthCheckPoint()
        {
            depthCheckPoint.Clear();
            depthCheckPoint.Add(AxisType.X, Vector3ExtensionMethod.CalcAxisBounds(AxisType.X, collider.bounds));
            depthCheckPoint.Add(AxisType.Y, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Y, collider.bounds));
            depthCheckPoint.Add(AxisType.Z, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Z, collider.bounds));
        }

        protected void Activate(bool active)
        {
            gameObject.SetActive(active);
            collider.enabled = active;
        }

        private void SynchronizationUnitPos(AxisType axis)
        {
            if (axis == AxisType.None)
            {
                CheckStandObject();
            }
            else
            {
                basicUnitInfo.LocalPos = transform.localPosition;
            }
        }
        
        private void CheckStandObject()
        {
            var origin = collider.transform.position;
            var dir = Vector3.down;

            var isHit = Physics.Raycast(origin, dir, out var hit, rayDistance, canStandMask);
        
            if (!isHit)
            {
                return;
            }

            var diff = hit.point - hit.transform.position;
            var standPos = hit.transform.localPosition + diff;
            standPos.SetAxisElement(converter.AxisType, basicUnitInfo.LocalPos.GetAxisElement(converter.AxisType));
            basicUnitInfo.LocalPos = standPos;
        }
    }
}