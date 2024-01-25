using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace StageStructureConvertSystem
{
    public class StructureConverter : MonoBehaviour
    {
        private List<StructureObjectUnitBase> _convertableUnits;

        private EAxisType _axisType;
        public EAxisType AxisType => _axisType;

        public bool Convertable { get; private set; }

        public void Init(Section section)
        {
            _axisType = EAxisType.NONE;
            SetConvertable(section is Stage);

            if (section is Stage)
            {
                _convertableUnits ??= new List<StructureObjectUnitBase>();
                _convertableUnits.Clear();
                section.GetComponentsInChildren(_convertableUnits);
                _convertableUnits.ForEach(unit => unit.Init(this));
            }
        }

        public void SetConvertable(bool convertable)
        {
            Convertable = convertable;
        }

        public void ConvertDimension(EAxisType axisType, Action callback = null)
        {
            if (!Convertable || _axisType == axisType || (_axisType != EAxisType.NONE && axisType != EAxisType.NONE))
            {
                callback?.Invoke();
                return;
            }

            Convertable = false;

            if(axisType == EAxisType.NONE)
            {
                ChangeAxis(axisType);
            }

            if (CameraManager.Instance.CurrentCamController is SectionCamController)
            {
                ((SectionCamController)CameraManager.Instance.CurrentCamController).ChangeCameraAxis(axisType, () =>
                {
                    if(axisType != EAxisType.NONE)
                    {
                        ChangeAxis(axisType);
                    }
            
                    Convertable = true;
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