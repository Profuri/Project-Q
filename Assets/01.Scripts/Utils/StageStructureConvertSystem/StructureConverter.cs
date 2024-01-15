using System;
using System.Collections.Generic;
using UnityEngine;
namespace StageStructureConvertSystem
{
    [RequireComponent(typeof(Stage))]
    public class StructureConverter : MonoBehaviour
    {
        private List<StructureObjectUnitBase> _convertableUnits;

        private EAxisType _axisType;
        public EAxisType AxisType => _axisType;

        private bool _isConvertable;

        public void Init()
        {
            _axisType = EAxisType.NONE;
            _isConvertable = true;
            _convertableUnits ??= new List<StructureObjectUnitBase>();
            _convertableUnits.Clear();
            GetComponentsInChildren(_convertableUnits);
            _convertableUnits.ForEach(unit => unit.Init(this));
        }

        public void SetConvertable(bool convertable)
        {
            _isConvertable = convertable;
        }

        public void ConvertDimension(EAxisType axisType, Action callback = null)
        {
            if (!_isConvertable || _axisType == axisType || (_axisType != EAxisType.NONE && axisType != EAxisType.NONE))
            {
                callback?.Invoke();
                return;
            }

            _isConvertable = false;

            if(axisType == EAxisType.NONE)
            {
                ChangeAxis(axisType);
            }

            if (CameraManager.Instance.CurrentCamController is StageCamController)
            {
                ((StageCamController)CameraManager.Instance.CurrentCamController).ChangeStageCamera(axisType, () =>
                {
                    if(axisType != EAxisType.NONE)
                    {
                        ChangeAxis(axisType);
                    }
            
                    _isConvertable = true;
                    callback?.Invoke();
                });
            }
        }

        private void ChangeAxis(EAxisType axisType)
        {
            CameraManager.Instance.ShakeCam(1f, 0.1f);
            VolumeManager.Instance.Highlight(0.2f);
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
        }

        public void RemoveObject(StructureObjectUnitBase unit)
        {
            Destroy(unit.gameObject);
            _convertableUnits.Remove(unit);
        }
    }
}