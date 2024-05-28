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

        private float _lastRotateValue;
        [SerializeField] private float _rotateValue = 45f;
        [SerializeField] private float _rotateTime = 0.5f;

        public override void EnterCam()
        {
            StartSafeCoroutine("CamRotateRoutine", CamRotateRoutine(_lastRotateValue = -45f, 0f));
            LightManager.Instance.RotateDefaultDirectionalLight(-30f, 0f);
            
            InputManager.Instance.CameraInputReader.OnRotateRightEvent += CamRotateRight;
            InputManager.Instance.CameraInputReader.OnRotateLeftEvent += CamRotateLeft;
            
            base.EnterCam();
        }

        public override void ExitCam()
        {
            InputManager.Instance.CameraInputReader.OnRotateRightEvent -= CamRotateRight;
            InputManager.Instance.CameraInputReader.OnRotateLeftEvent -= CamRotateLeft;
            
            base.ExitCam();
        }

        private void CamRotateRight()
        {
            StartSafeCoroutine("CamRotateRoutine", CamRotateRoutine(_lastRotateValue - _rotateValue, _rotateTime));
            LightManager.Instance.RotateDefaultDirectionalLight(_lastRotateValue - (_rotateValue - 15f), _rotateTime);
            _lastRotateValue -= _rotateValue;
        }
        
        private void CamRotateLeft()
        {
            StartSafeCoroutine("CamRotateRoutine", CamRotateRoutine(_lastRotateValue + _rotateValue, _rotateTime));
            LightManager.Instance.RotateDefaultDirectionalLight(_lastRotateValue + (_rotateValue + 15f), _rotateTime);
            _lastRotateValue += _rotateValue;
        }

        private IEnumerator CamRotateRoutine(float rotateValue, float time)
        {
            var currentRot = transform.localRotation;

            var localEulerAngle = transform.localEulerAngles;
            var targetRot = Quaternion.Euler(localEulerAngle.x, rotateValue, localEulerAngle.z);

            var currentTime = 0f;
            while (currentTime <= time)
            {
                currentTime += Time.deltaTime;
                var percent = currentTime / time;

                var rot = Quaternion.Lerp(currentRot, targetRot, percent);
                transform.localRotation = rot;
                yield return null;
            }

            transform.localRotation = targetRot;
        }
    }
}