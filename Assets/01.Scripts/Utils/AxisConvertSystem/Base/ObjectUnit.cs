using System;
using System.Collections.Generic;
using UnityEngine;

namespace AxisConvertSystem
{
    public class ObjectUnit : MonoBehaviour
    {
        [SerializeField] private CompressLayer _compressLayer;
        [SerializeField] private bool _reloadOnCollisionToObstacle = false;
        [SerializeField] private bool _staticObject = false;

        protected AxisConverter Converter { get; private set; }
        protected Collider Collider { get; private set; }

        private ObjectInfo _initInfo;
        private ObjectInfo _prevInfo;
        private ObjectInfo _currentInfo;

        private Dictionary<AxisType, List<Vector3>> _depthCheckPoint;
        private float _depth;

        public virtual void Init(AxisConverter converter)
        {
            Converter = converter;
            Collider = GetComponent<Collider>();
            CalcDepthCheckPoint();
        }

        public virtual void ConvertDimension(AxisType axis)
        {
            if (axis == AxisType.None)
            {
                return;
            }
            
            CalcDepth(axis);
            Debug.Log(_depth);
        }

        private void CalcDepthCheckPoint()
        {
            _depthCheckPoint ??= new Dictionary<AxisType, List<Vector3>>();
            _depthCheckPoint.Clear();
            
            _depthCheckPoint.Add(AxisType.X, Vector3ExtensionMethod.CalcAxisBounds(AxisType.X, Collider.bounds));
            _depthCheckPoint.Add(AxisType.Y, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Y, Collider.bounds));
            _depthCheckPoint.Add(AxisType.Z, Vector3ExtensionMethod.CalcAxisBounds(AxisType.Z, Collider.bounds));
        }

        private void CalcDepth(AxisType axis)
        {
            if (!_staticObject)
            {
                CalcDepthCheckPoint();
            }
            _depth = 0;
            
            for (var i = 0; i < 4; i++)
            {
                var isHit = Physics.Raycast(_depthCheckPoint[axis][i], Vector3ExtensionMethod.GetAxisDir(axis),
                    out var hit, Mathf.Infinity, Converter.ObjectMask);
                var depth = isHit ? Mathf.Abs(hit.point.GetAxisElement(axis)) : float.MaxValue;
                
                _depth = Mathf.Max(_depth, depth);
            }
        }

        private void OnDrawGizmos()
        {
            if (_depthCheckPoint == null)
            {
                return;
            }
            
            Gizmos.color = Color.green;
            for (var i = 0; i < 4; i++)
            {
                Gizmos.DrawRay(_depthCheckPoint[AxisType.X][i], Vector3ExtensionMethod.GetAxisDir(AxisType.X) * 3);
            }
            
            Gizmos.color = Color.red;
            for (var i = 0; i < 4; i++)
            {
                Gizmos.DrawRay(_depthCheckPoint[AxisType.Y][i], Vector3ExtensionMethod.GetAxisDir(AxisType.Y) * 3);
            }
            
            Gizmos.color = Color.blue;
            for (var i = 0; i < 4; i++)
            {
                Gizmos.DrawRay(_depthCheckPoint[AxisType.Z][i], Vector3ExtensionMethod.GetAxisDir(AxisType.Z) * 3);
            }
        }
    }
}