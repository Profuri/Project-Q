using System;
using System.Collections.Generic;
using System.Linq;

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
            Depth = float.MaxValue;
        }

        public void CalcDepth(AxisType axis)
        {
            Depth = float.MaxValue;
            
            if (axis == AxisType.None || _owner.IsHide)
            {
                return;
            }

            if (_owner.Section.SectionUnits.Where(unit => unit != _owner).Any(
                    unit => _depthCheckPoint[axis].Block(unit.DepthHandler._depthCheckPoint[axis])))
            {
                Depth = 0f;
            }
        }

        public void DepthCheckPointSetting()
        {
            if (!_owner.activeUnit)
            {
                _owner.Activate(true);
            }

            _depthCheckPoint.Clear();

            foreach (AxisType axis in Enum.GetValues(typeof(AxisType)))
            {
                var depthPoint = _owner.Collider.GetDepthPoint(axis);
                _depthCheckPoint.Add(axis, depthPoint);
            }

            _owner.Activate(_owner.activeUnit);
        }
    }
}