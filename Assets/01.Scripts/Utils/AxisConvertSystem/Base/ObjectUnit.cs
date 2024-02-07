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
            collider = compressLayer == CompressLayer.Player ?
                GetComponent<Collider>() : transform.Find("Collider").GetComponent<Collider>();
            if (!staticUnit)
            {
                rigid = GetComponent<Rigidbody>();
            }
            
            originUnitInfo = new UnitInfo
            {
                LocalPos = transform.localPosition,
                LocalRot = transform.localRotation,
                LocalScale = transform.localScale,
                ColliderCenter = Vector3.zero
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

        public virtual void Convert(AxisType axis, UnitInfo? info = default)
        {
            if (!staticUnit)
            {
                RenewalBasicUnitInfo(axis);
            }
            
            var unitInfo = ConvertInfo(info ?? basicUnitInfo, axis);
            UnitSetting(axis, unitInfo);   

            Activate(Math.Abs(depth - float.MaxValue) < 0.01f);
        }

        protected virtual void UnitSetting(AxisType axis, UnitInfo unitInfo)
        {
            transform.localPosition = unitInfo.LocalPos;
            transform.localRotation = unitInfo.LocalRot;
            transform.localScale = unitInfo.LocalScale;
            // collider.transform.localPosition = unitInfo.ColliderCenter;
        }

        protected UnitInfo ConvertInfo(UnitInfo basic, AxisType axis)
        {
            if (axis == AxisType.None)
            {
                return basic;
            }

            var info = new UnitInfo
            {
                LocalPos = basic.LocalPos,
                LocalRot = basic.LocalRot,
                LocalScale = basic.LocalScale,
                ColliderCenter = basic.ColliderCenter
            };

            info.LocalScale.SetAxisElement(axis, 1);
            info.LocalPos.SetAxisElement(axis, (int)compressLayer * Vector3ExtensionMethod.GetAxisDir(axis).GetAxisElement(axis));
            info.ColliderCenter.SetAxisElement(axis, -(int)compressLayer * Vector3ExtensionMethod.GetAxisDir(axis).GetAxisElement(axis));

            return info;
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
            collider.enabled = active;
        }

        private void RenewalBasicUnitInfo(AxisType axis)
        {
            Debug.Log(2);
            if (axis == AxisType.None)
            {
                CheckStandObject(axis);
            }
            else
            {
                basicUnitInfo.LocalPos = transform.localPosition;
                basicUnitInfo.LocalRot = transform.localRotation;
                basicUnitInfo.LocalScale = transform.localScale;
                basicUnitInfo.ColliderCenter = Vector3.zero;
            }
        }
        
        private void CheckStandObject(AxisType axisType)
        {
            var origin = collider.transform.position;
            var dir = Vector3.down;
        
            var isHit = Physics.Raycast(origin, dir, out var hit, rayDistance, canStandMask);
        
            if (!isHit)
            {
                return;
            }

            Debug.Log(hit.point);
            basicUnitInfo.LocalPos = hit.point;
        }
    }
}