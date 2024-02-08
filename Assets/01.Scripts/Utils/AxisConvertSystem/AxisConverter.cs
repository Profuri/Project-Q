using System;
using System.Collections.Generic;
using UnityEngine;

namespace AxisConvertSystem
{
    public class AxisConverter : MonoBehaviour
    {
        [SerializeField] private LayerMask _objectMask;
        public LayerMask ObjectMask => _objectMask;
        
        private List<ObjectUnit> _convertableUnits;

        private AxisType _axisType;
        public AxisType AxisType => _axisType;

        public bool Convertable { get; private set; }

        public void Init(Section section)
        {
            _axisType = AxisType.None;
            SetConvertable(section is Stage);

            if (section is Stage)
            {
                _convertableUnits ??= new List<ObjectUnit>();
                _convertableUnits.Clear();
                section.GetComponentsInChildren(_convertableUnits);
                _convertableUnits.ForEach(unit => unit.Init(this));
            }
        }

        public void SetConvertable(bool convertable)
        {
            Convertable = convertable;
        }

        public void ConvertDimension(AxisType axisType, Action callback = null)
        {
            if (!Convertable || _axisType == axisType || (_axisType != AxisType.None && axisType != AxisType.None))
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
            
            _convertableUnits.ForEach(unit =>
            {
                unit.CalcDepth(axisType);
            });
            
            _convertableUnits.ForEach(unit =>
            {
                unit.Convert(axisType);
            });
            
            _axisType = axisType;
        }

        public void RemoveObject(ObjectUnit unit)
        {
            Destroy(unit.gameObject);
            _convertableUnits.Remove(unit);
        }
    }
}