using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace AxisConvertSystem
{
    public class ObjectUnit : MonoBehaviour
    {
        [SerializeField] private CompressLayer _compressLayer;
        [SerializeField] private bool _staticObject = true;
        
        [Header("For Dynamic Object")]
        [SerializeField] private LayerMask _standableObjectMask;
        [SerializeField] private float _rayDistance;

        protected AxisConverter Converter { get; private set; }
        protected Collider ColliderCompo { get; private set; }
        protected Rigidbody RigidbodyCompo { get; private set; }

        private UnitInfo _originUnitInfo;
        private UnitInfo _basicUnitInfo;
        
        private Dictionary<AxisType, List<Vector3>> _depthCheckPoint;

        private float _depth;

        public virtual void Init(AxisConverter converter)
        {
            Converter = converter;
            ColliderCompo = GetComponent<Collider>();
            if (!_staticObject)
            {
                RigidbodyCompo = GetComponent<Rigidbody>();
            }
            
            _originUnitInfo = new UnitInfo
            {
                LocalPos = transform.localPosition,
                LocalRot = transform.localRotation,
                LocalScale = transform.localScale,
                ColliderCenter = ColliderCompo.bounds.center
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
            
            if (!_staticObject)
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

        public void Convert(AxisType axis, UnitInfo? info = default)
        {
            var unitInfo = ConvertInfo(info ?? _basicUnitInfo, axis);
            
            transform.localPosition = unitInfo.LocalPos;
            transform.localRotation = unitInfo.LocalRot;
            transform.localScale = unitInfo.LocalScale;
            var bounds = ColliderCompo.bounds;
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
            info.LocalPos.SetAxisElement(type, (int)_compressLayer);
            info.ColliderCenter.SetAxisElement(type, -(int)_compressLayer);

            return info;
        }

        private void CalcDepthCheckPoint()
        {
            _depthCheckPoint.Clear();
            _depthCheckPoint.Add(AxisType.X, Vector3ExtensionMethod.CalcAxisBounds(AxisType.X, ColliderCompo.bounds));
            _depthCheckPoint.Add(AxisType.Y, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Y, ColliderCompo.bounds));
            _depthCheckPoint.Add(AxisType.Z, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Z, ColliderCompo.bounds));
        }

        private void Activate(bool active)
        {
            ColliderCompo.enabled = active;
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
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_staticObject)
            {
                if (!TryGetComponent<Rigidbody>(out var rigid))
                {
                    transform.AddComponent<Rigidbody>();
                }
            }
            else
            {
                if (TryGetComponent<Rigidbody>(out var rigid))
                {
                    UnityEditor.EditorApplication.delayCall+=()=>
                    {
                        DestroyImmediate(rigid);
                    };
                }
            }
        }
#endif
    }
}