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
                if (unit == _owner || unit.renderType == UnitRenderType.Transparent)
                {
                    continue;
                }

                if (_depthCheckPoint[axis].Block(unit.DepthHandler._depthCheckPoint[axis]))
                {
                    Depth = 0f;
                    Debug.Log(unit.name);
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