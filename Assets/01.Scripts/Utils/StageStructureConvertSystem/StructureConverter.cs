using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageStructureConvertSystem
{
    public class StructureConverter : MonoBehaviour
    {
        private List<StructureObjectUnit> _convertableUnits;

        private EAxisType _axisType;
        public EAxisType AxisType => _axisType;

        private void Awake()
        {
            _axisType = EAxisType.NONE;
            _convertableUnits = new List<StructureObjectUnit>();
            GetComponentsInChildren(_convertableUnits);
            _convertableUnits.ForEach(unit => unit.Init());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                ConvertDimension(EAxisType.NONE);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                ConvertDimension(EAxisType.X);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                ConvertDimension(EAxisType.Y);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                ConvertDimension(EAxisType.Z);
            }
        }

        public void ConvertDimension(EAxisType axisType)
        {
            if (_axisType == axisType)
                return;

            if (_axisType != EAxisType.NONE && axisType != EAxisType.NONE)
                return;
            
            _axisType = axisType;
            _convertableUnits.ForEach(unit =>
            {
                unit.ConvertDimension(axisType);
            });
        }
    }
}