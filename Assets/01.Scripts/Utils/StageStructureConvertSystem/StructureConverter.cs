using System.Collections.Generic;
using UnityEngine;

namespace StageStructureConvertSystem
{
    public class StructureConverter : MonoBehaviour
    {
        private List<StructureObjectUnitBase> _convertableUnits;

        private EAxisType _axisType;
        public EAxisType AxisType => _axisType;

        private void Awake()
        {
            _axisType = EAxisType.NONE;
            _convertableUnits = new List<StructureObjectUnitBase>();
            GetComponentsInChildren(_convertableUnits);
            _convertableUnits.ForEach(unit => unit.Init());
        }

        public void ConvertDimension(EAxisType axisType)
        {
            if (_axisType == axisType)
                return;

            if (_axisType != EAxisType.NONE && axisType != EAxisType.NONE)
                return;
            
            CameraManager.Instance.ChangeCamera(axisType, () =>
            {
                CameraManager.Instance.ShakeCam();
                VolumeManager.Instance.Highlight(0.2f);
                CameraManager.Instance.SetOrthographic(axisType != EAxisType.NONE);
                LightManager.Instance.SetShadow(axisType == EAxisType.NONE ? LightShadows.Soft : LightShadows.None);
                
                _axisType = axisType;

                _convertableUnits.ForEach(unit =>
                {
                    unit.ConvertDimension(axisType);
                });
                
                _convertableUnits.ForEach(unit =>
                {
                    unit.TransformSynchronization(axisType);
                });
                
                _convertableUnits.ForEach(unit =>
                {
                    unit.ObjectSetting();
                });
            });

        }
    }
}