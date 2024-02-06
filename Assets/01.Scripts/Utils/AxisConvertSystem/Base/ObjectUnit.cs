using System;
using System.Collections.Generic;
using UnityEngine;

namespace AxisConvertSystem
{
    [Serializable]
    public class UnitCustomInspector
    {
        [HideInInspector] public CompressLayer compressLayer;
        [HideInInspector] public bool staticUnit = true;
        
        [HideInInspector] public LayerMask canStandMask;
        [HideInInspector] public float rayDistance;
    }
    
    public class ObjectUnit : MonoBehaviour
    {
        public UnitCustomInspector customInspector;

        private AxisConverter _converter;
        private Collider _collider;
        private Rigidbody _rigid;

        private UnitInfo _originUnitInfo;
        private UnitInfo _basicUnitInfo;
        
        private Dictionary<AxisType, List<Vector3>> _depthCheckPoint;

        private float _depth;

        public virtual void Init(AxisConverter converter)
        {
            _converter = converter;

            // if (!transform.Find("Collider"))
            // {
            //     
            // }
            //
            // _collider = transform.Find("Collider").GetComponent<Collider>();
            //
            // if (!_staticObject)
            // {
            //     _rigid = TryGetComponent<Rigidbody>(out var rigid) ? rigid : transform.AddComponent<Rigidbody>();
            // }
            
            _originUnitInfo = new UnitInfo
            {
                LocalPos = transform.localPosition,
                LocalRot = transform.localRotation,
                LocalScale = transform.localScale,
                ColliderCenter = _collider.bounds.center
            };
            _basicUnitInfo = _originUnitInfo;
            
            _depthCheckPoint ??= new Dictionary<AxisType, List<Vector3>>();

            CalcDepthCheckPoint();
        }

        public void CalcDepth(AxisType axis)
        {
            if (axis == AxisType.None)
            {
                _depth = float.MaxValue;
                return;
            }
            
            if (!customInspector.staticUnit)
            {
                CalcDepthCheckPoint();
            }
            _depth = 0f;
            
            for (var i = 0; i < 4; i++)
            {
                var isHit = Physics.Raycast(_depthCheckPoint[axis][i], Vector3ExtensionMethod.GetAxisDir(axis),
                    out var hit, Mathf.Infinity, _converter.ObjectMask);
                var temp = isHit ? Mathf.Abs(hit.point.GetAxisElement(axis)) : float.MaxValue;
                
                _depth = Mathf.Max(_depth, temp);
            }
        }

        public void Convert(AxisType axis, UnitInfo? info = default)
        {
            var unitInfo = ConvertInfo(info ?? _basicUnitInfo, axis);
            
            transform.localPosition = unitInfo.LocalPos;
            transform.localRotation = unitInfo.LocalRot;
            transform.localScale = unitInfo.LocalScale;
            var bounds = _collider.bounds;
            bounds.center = unitInfo.ColliderCenter;

            Activate(Math.Abs(_depth - float.MaxValue) < 0.01f);
        }

        private UnitInfo ConvertInfo(UnitInfo basic, AxisType type)
        {
            if (type == AxisType.None)
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

            info.LocalScale.SetAxisElement(type, 1);
            info.LocalPos.SetAxisElement(type, (int)customInspector.compressLayer);
            info.ColliderCenter.SetAxisElement(type, -(int)customInspector.compressLayer);

            return info;
        }

        private void CalcDepthCheckPoint()
        {
            _depthCheckPoint.Clear();
            _depthCheckPoint.Add(AxisType.X, Vector3ExtensionMethod.CalcAxisBounds(AxisType.X, _collider.bounds));
            _depthCheckPoint.Add(AxisType.Y, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Y, _collider.bounds));
            _depthCheckPoint.Add(AxisType.Z, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Z, _collider.bounds));
        }

        private void Activate(bool active)
        {
            _collider.enabled = active;
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