using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

namespace VirtualCam
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class VirtualCamComponent : MonoBehaviour, IVirtualCam
    {
        private CinemachineVirtualCameraBase _virtualCam;

        public event Action OnEnter = null;
        public event Action OnUpdate = null;
        public event Action OnExit = null;

        private Coroutine _runningRoutine = null;

        private void Awake()
        {
            _virtualCam = GetComponent<CinemachineVirtualCameraBase>();
        }

        public void EnterCam()
        {
            _virtualCam.m_Priority = 10;
            OnEnter?.Invoke();
        }

        public void UpdateCam()
        {
            OnUpdate?.Invoke();
        }

        public void ExitCam()
        {
            _virtualCam.m_Priority = 0;
            OnExit?.Invoke();
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