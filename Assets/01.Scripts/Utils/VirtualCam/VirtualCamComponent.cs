using System.Collections;
using Cinemachine;
using UnityEngine;

namespace VirtualCam
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class VirtualCamComponent : MonoBehaviour, IVirtualCam
    {
        private CinemachineVirtualCameraBase _virtualCam;

        private Coroutine _runningRoutine = null;

        private void Awake()
        {
            _virtualCam = GetComponent<CinemachineVirtualCameraBase>();
        }

        public void EnterCam()
        {
            _virtualCam.m_Priority = 10;
        }

        public void ExitCam()
        {
            _virtualCam.m_Priority = 0;
        }

        public void SetFollowTarget(Transform followTarget)
        {
            _virtualCam.Follow = followTarget;
        }

        public void SetLookAtTarget(Transform lookAtTarget)
        {
            _virtualCam.LookAt = lookAtTarget;
        }

        public void ShakeCam(float intensity, float time)
        {
            if (_runningRoutine != null)
            {
                StopCoroutine(_runningRoutine);
                _runningRoutine = null;
            }
            _runningRoutine = StartCoroutine(ShakeSequence(intensity, time));
        }

        public void SetAxisXValue(float value)
        {
            if (_virtualCam is not CinemachineVirtualCamera vCam)
            {
                Debug.LogError("[VirtualCamCompo] This cam is cant have y value. return 0");
                return;
            }
            
            var pov = vCam.GetCinemachineComponent<CinemachinePOV>();

            if (pov is null)
            {
                transform.Rotate(0, value, 0);   
            }
            else
            {
                pov.m_HorizontalAxis.Value = value;
            }
        }

        public float GetAxisXValue()
        {
            if (_virtualCam is not CinemachineVirtualCamera vCam)
            {
                Debug.LogError("[VirtualCamCompo] This cam is cant have y value. return 0");
                return 0f;
            }
            
            var pov = vCam.GetCinemachineComponent<CinemachinePOV>();
            return pov is null ? -(360 - transform.localEulerAngles.y) : pov.m_HorizontalAxis.Value;
        }
        
        public void SetAxisYValue(float value)
        {
            if (_virtualCam is not CinemachineVirtualCamera vCam)
            {
                Debug.LogError("[VirtualCamCompo] This cam is cant have y value. return 0");
                return;
            }
            
            var pov = vCam.GetCinemachineComponent<CinemachinePOV>();
            
            if (pov is null)
            {
                transform.Rotate(value, 0, 0);   
            }
            else
            {
                pov.m_VerticalAxis.Value = value;
            }
        }

        public float GetAxisYValue()
        {
            if (_virtualCam is not CinemachineVirtualCamera vCam)
            {
                Debug.LogError("[VirtualCamCompo] This cam is cant have y value. return 0");
                return 0f;
            }
            
            var pov = vCam.GetCinemachineComponent<CinemachinePOV>();
            return pov is null ? transform.localEulerAngles.x : pov.m_VerticalAxis.Value;
        }

        private IEnumerator ShakeSequence(float intensity, float time)
        {
            yield return StartCoroutine(ShakeRoutine(intensity, time / 2f));
            yield return StartCoroutine(ShakeRoutine(-intensity, time / 2f));
            _runningRoutine = null;
        }

        private IEnumerator ShakeRoutine(float intensity, float time)
        {
            if (_virtualCam is not CinemachineVirtualCamera virtualCamera) 
                yield break;
            
            var origin = virtualCamera.m_Lens.OrthographicSize;

            var currentTime = 0f;
            var percent = 0f;

            while (percent < 1f)
            {
                currentTime += Time.deltaTime;
                percent = CameraManager.Instance.CamShakeCurve.Evaluate(currentTime / time);
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(origin, origin + intensity, percent);
                yield return null;
            }
        }
    }
}