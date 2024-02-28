using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AxisConvertSystem
{
    public class UnitDepthHandler
    {
        private readonly ObjectUnit _owner;
        private readonly Dictionary<AxisType, DepthPoint> _depthCheckPoint;
        
        public float Depth { get; private set; }

        public UnitDepthHandler(ObjectUnit owner)
        {
            _owner = owner;
            _depthCheckPoint = new Dictionary<AxisType, DepthPoint>();
        }

        public void CalcDepth(AxisType axis)
        {
            Depth = float.MaxValue;
            
            if (axis == AxisType.None)
            {
                return;
            }
            
            if (!_owner.staticUnit)
            {
                DepthCheckPointSetting();
            }

            if (_owner.Section.SectionUnits.Where(unit => unit != _owner).Any(
                    unit => _depthCheckPoint[axis].Block(unit.DepthHandler._depthCheckPoint[axis])))
            {
                Depth = 0f;
            }
        }

        public void DepthCheckPointSetting()
        {
            _depthCheckPoint.Clear();

            foreach (AxisType axis in Enum.GetValues(typeof(AxisType)))
            {
                var depthPoint = _owner.Collider.GetDepthPoint(axis);
                _depthCheckPoint.Add(axis, depthPoint);
            }
        }
    }
}