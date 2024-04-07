using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace VirtualCam
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class VirtualCamComponent : MonoBehaviour, IVirtualCam
    {
        private CinemachineVirtualCamera _virtualCam;

        private void Awake()
        {
            _virtualCam = GetComponent<CinemachineVirtualCamera>();
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

        public void ApplyOffset(Vector3 offset, float time)
        {
            CoroutineManager.Instance.StartCoroutine(GetInstanceID(), OffsetChangeRoutine(offset, time));
        }

        public void ShakeCam(float intensity, float time)
        {
            CoroutineManager.Instance.StartCoroutine(GetInstanceID(), ShakeSequence(intensity, time));
        }

        public void SetAxisXValue(float value)
        {
            var pov = _virtualCam.GetCinemachineComponent<CinemachinePOV>();

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
            var pov = _virtualCam.GetCinemachineComponent<CinemachinePOV>();
            return pov is null ? -(360 - transform.localEulerAngles.y) : pov.m_HorizontalAxis.Value;
        }
        
        public void SetAxisYValue(float value)
        {
            var pov = _virtualCam.GetCinemachineComponent<CinemachinePOV>();
            
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
            var pov = _virtualCam.GetCinemachineComponent<CinemachinePOV>();
            return pov is null ? transform.localEulerAngles.x : pov.m_VerticalAxis.Value;
        }

        private IEnumerator ShakeSequence(float intensity, float time)
        {
            yield return StartCoroutine(ShakeRoutine(intensity, time / 2f));
            yield return StartCoroutine(ShakeRoutine(-intensity, time / 2f));
        }

        private IEnumerator ShakeRoutine(float intensity, float time)
        {
            var origin = _virtualCam.m_Lens.OrthographicSize;

            var currentTime = 0f;
            var percent = 0f;

            while (percent < 1f)
            {
                currentTime += Time.deltaTime;
                percent = CameraManager.Instance.CamShakeCurve.Evaluate(currentTime / time);
                _virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(origin, origin + intensity, percent);
                yield return null;
            }
        }

        private IEnumerator OffsetChangeRoutine(Vector3 targetOffset, float time)
        {
            var currentTime = 0f;
            var percent = 0f;
            
            var framingTransposer = _virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            var originOffset = framingTransposer.m_TrackedObjectOffset;
            var distance = Vector3.Distance(originOffset, targetOffset);

            time *= distance / CameraManager.Instance.OffsetAmount;
            
            while (percent < 1f)
            {
                currentTime += Time.deltaTime;
                percent = CameraManager.Instance.CamOffsetCurve.Evaluate(currentTime / time);
                var offset = Vector3.Lerp(originOffset, targetOffset, percent);
                framingTransposer.m_TrackedObjectOffset = offset;
                yield return null;
            }
            
            framingTransposer.m_TrackedObjectOffset = targetOffset;
        }
    }
}