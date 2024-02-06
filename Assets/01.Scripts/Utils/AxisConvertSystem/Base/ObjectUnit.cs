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
            var unitInfo = ConvertInfo(info ?? basicUnitInfo, axis);
            
            transform.localPosition = unitInfo.LocalPos;
            transform.localRotation = unitInfo.LocalRot;
            transform.localScale = unitInfo.LocalScale;
            collider.transform.localPosition = unitInfo.ColliderCenter;

            Activate(Math.Abs(depth - float.MaxValue) < 0.01f);
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
        
        // private void CheckStandObject(AxisType axisType)
        // {
        //     var origin = _prevObjectInfo.LocalPos + Vector3.up * (_prevObjectInfo.LocalScale.y / 2f);
        //     var dir = Vector3.down;
        //
        //     var isHit = Physics.Raycast(origin, dir, out var hit, _rayDistance, _standableObjectMask);
        //
        //     if (!isHit)
        //     {
        //         return;
        //     }
        //
        //     if (!hit.collider.TryGetComponent<ObjectUnit>(out var unit))
        //     {
        //         return;
        //     }
        //
        //     switch (axisType)
        //     {
        //         case AxisType.X:
        //             if (_objectInfo.LocalPos.x >= unit.ObjectInfo.LocalPos.x - unit.ObjectInfo.LocalScale.x / 2f  &&
        //                 _objectInfo.LocalPos.x <= unit.ObjectInfo.LocalPos.x + unit.ObjectInfo.LocalScale.x / 2f)
        //             {
        //                 return;
        //             }
        //             _objectInfo.LocalPos.x = unit.ObjectInfo.LocalPos.x;
        //             break;
        //         case AxisType.Y:
        //             if (_objectInfo.LocalPos.y >= unit.ObjectInfo.LocalPos.y + unit.ObjectInfo.LocalScale.y / 2f + _objectInfo.LocalScale.y / 2f)
        //             {
        //                 return;
        //             }
        //             _objectInfo.LocalPos.y = unit.ObjectInfo.LocalPos.y + unit.ObjectInfo.LocalScale.y / 2f + _objectInfo.LocalScale.y / 2f;
        //             break;
        //         case AxisType.Z:
        //             if (_objectInfo.LocalPos.z >= unit.ObjectInfo.LocalPos.z - unit.ObjectInfo.LocalScale.z / 2f  &&
        //                 _objectInfo.LocalPos.z <= unit.ObjectInfo.LocalPos.z + unit.ObjectInfo.LocalScale.z / 2f)
        //             {
        //                 return;
        //             }
        //             _objectInfo.LocalPos.z = unit.ObjectInfo.LocalPos.z;
        //             break;
        //     }
        // }
    }
}