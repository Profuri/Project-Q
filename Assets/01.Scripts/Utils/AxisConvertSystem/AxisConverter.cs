using System;
using System.Collections.Generic;
using UnityEngine;

namespace AxisConvertSystem
{
    public class AxisConverter : MonoBehaviour
    {
        [SerializeField] private LayerMask _objectMask;
        public LayerMask ObjectMask => _objectMask;

        public List<ObjectUnit> Units { get; private set; }
        public AxisType AxisType { get; private set; }

        public bool Convertable { get; private set; }

        public void Init(Section section)
        {
            AxisType = AxisType.None;
            SetConvertable(section is Stage);

            if (section is Stage)
            {
                Units ??= new List<ObjectUnit>();
                Units.Clear();
                section.GetComponentsInChildren(Units);
                Units.ForEach(unit => unit.Init(this));
            }
        }

        public void SetConvertable(bool convertable)
        {
            Convertable = convertable;
        }

        public void ConvertDimension(AxisType axisType, Action callback = null)
        {
            if (!Convertable || AxisType == axisType || (AxisType != AxisType.None && axisType != AxisType.None))
            {
                callback?.Invoke();
                return;
            }

            Convertable = false;

            if(axisType == AxisType.None)
            {
                ChangeAxis(axisType);
            }

            if (CameraManager.Instance.CurrentCamController is SectionCamController)
            {
                ((SectionCamController)CameraManager.Instance.CurrentCamController).ChangeCameraAxis(axisType, () =>
                {
                    if(axisType != AxisType.None)
                    {
                        ChangeAxis(axisType);
                    }
            
                    Convertable = true;
                    callback?.Invoke();
                });
            }
        }

        private void ChangeAxis(AxisType axisType)
        {
            CameraManager.Instance.ShakeCam(1f, 0.1f);
            VolumeManager.Instance.Highlight(0.2f);
            LightManager.Instance.SetShadow(axisType == AxisType.None ? LightShadows.Soft : LightShadows.None);

            Units.ForEach(unit =>
            {
                unit.Convert(axisType);
            });
            
            Units.ForEach(unit =>
            {
                unit.UnitSetting(axisType);
            });

            AxisType = axisType;
        }
    }
}