using System;
using System.Collections.Generic;
using UnityEngine;

namespace AxisConvertSystem
{
    public class UnitDepthHandler
    {
        private readonly ObjectUnit _owner;
        private readonly Dictionary<AxisType, DepthPoint> _depthCheckPoint;

        public float Depth { get; private set; }
        public bool Hide => Mathf.Abs(Depth - float.MaxValue) >= 0.01f;

        public UnitDepthHandler(ObjectUnit owner)
        {
            _owner = owner;
            _depthCheckPoint = new Dictionary<AxisType, DepthPoint>();
            InitDepth();
        }

        public float GetDepth(AxisType axis)
        {
            if (axis == AxisType.None)
            {
                return 0;
            }

            var depthPoint = _depthCheckPoint[axis];
            return depthPoint.Z;
        }

        public void InitDepth()
        {
            Depth = float.MaxValue;
        }

        public void CalcDepth(AxisType axis)
        {
            InitDepth();
            
            if (axis == AxisType.None || _owner.IsHide)
            {
                return;
            }

            foreach (var unit in _owner.Section.SectionUnits)
            {
                var isOwner = unit == _owner;
                var isTransparent = unit.renderType == UnitRenderType.Transparent;
                var isSuperior = _owner.subUnit && _owner.IsSuperiorUnit(unit);
                var isChild = !_owner.subUnit && _owner.IsChildUnit(unit);
                
                if (isOwner || isTransparent || isSuperior || isChild)
                {
                    continue;
                }

                var excludeLayer = (_owner.Collider.excludeLayers & (1 << unit.gameObject.layer)) != 0;
                if (!excludeLayer && _depthCheckPoint[axis].Intersect(unit.DepthHandler._depthCheckPoint[axis]))
                {
                    unit.IntersectedUnits.Add(_owner);
                }

                var isDynamic = !unit.staticUnit;
                if (!isDynamic && _depthCheckPoint[axis].Block(unit.DepthHandler._depthCheckPoint[axis]))
                {
                    Depth = 0f;
                    break;
                }
            }
        }

        public DepthPoint GetDepthPoint(AxisType axis)
        {
            return _depthCheckPoint[axis];
        }

        public void DepthCheckPointSetting()
        {
            var prevActive = _owner.activeUnit;
            
            if (!prevActive)
            {
                _owner.Activate(true);
            }

            _depthCheckPoint.Clear();

            foreach (AxisType axis in Enum.GetValues(typeof(AxisType)))
            {
                var depthPoint = _owner.Collider.GetDepthPoint(axis);
                _depthCheckPoint.Add(axis, depthPoint);
            }

            _owner.Activate(prevActive);
        }
    }
}