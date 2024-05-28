using System.Collections;
using AxisConvertSystem;
using UnityEngine;

namespace VirtualCam
{
    public class SectionVirtualCam : VirtualCamComponent
    {
        [SerializeField] private bool _isControlCam;
        public bool IsControlCam => _isControlCam;
        
        [SerializeField] private AxisType _axisType;
        public AxisType AxisType => _axisType;

        public override void EnterCam()
        {
            if (_axisType == AxisType.None)
            {
                var localEuler = transform.localEulerAngles;
                localEuler.y = CameraManager.Instance.LastRotateValue;
                transform.localEulerAngles = localEuler;

                InputManager.Instance.CameraInputReader.OnRotateRightEvent += CamRotateRight;
                InputManager.Instance.CameraInputReader.OnRotateLeftEvent += CamRotateLeft;
            }
            
            base.EnterCam();
        }

        public override void ExitCam()
        {
            if (_axisType == AxisType.None)
            {
                InputManager.Instance.CameraInputReader.OnRotateRightEvent -= CamRotateRight;
                InputManager.Instance.CameraInputReader.OnRotateLeftEvent -= CamRotateLeft;
            }

            base.ExitCam();
        }

        private void CamRotateRight()
        {
            if (SceneControlManager.Instance.CurrentScene.Type != SceneType.Stage)
            {
                return;
            }

            RotateCam(CameraManager.Instance.LastRotateValue - CameraManager.Instance.RotateValue, CameraManager.Instance.RotateTime);
            LightManager.Instance.RotateDefaultDirectionalLight(CameraManager.Instance.LastRotateValue - (CameraManager.Instance.RotateValue - 15f), CameraManager.Instance.RotateTime);
            CameraManager.Instance.LastRotateValue -= CameraManager.Instance.RotateValue;
        }
        
        private void CamRotateLeft()
        {
            if (SceneControlManager.Instance.CurrentScene.Type != SceneType.Stage)
            {
                return;
            }
            
            RotateCam(CameraManager.Instance.LastRotateValue + CameraManager.Instance.RotateValue, CameraManager.Instance.RotateTime);
            LightManager.Instance.RotateDefaultDirectionalLight(CameraManager.Instance.LastRotateValue + (CameraManager.Instance.RotateValue + 15f), CameraManager.Instance.RotateTime);
            CameraManager.Instance.LastRotateValue += CameraManager.Instance.RotateValue;
        }
    }
}