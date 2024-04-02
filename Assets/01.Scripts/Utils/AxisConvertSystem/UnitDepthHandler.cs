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
                var isParentUnit = _owner.subUnit && unit.GetParentUnit() == _owner;
                var isDynamic = !unit.staticUnit;
                
                if (isOwner || isTransparent || isParentUnit || isDynamic)
                {
                    continue;
                }

                if (_depthCheckPoint[axis].Block(unit.DepthHandler._depthCheckPoint[axis]))
                {
                    Depth = 0f;
                    break;
                }
            }
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