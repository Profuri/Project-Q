using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

namespace VirtualCam
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class VirtualCamComponent : ExtendedMono, IVirtualCam
    {
        private CinemachineVirtualCamera _virtualCam;
        private float _originOrthoSize;

        protected virtual void Awake()
        {
            _virtualCam = GetComponent<CinemachineVirtualCamera>();
            _originOrthoSize = _virtualCam.m_Lens.OrthographicSize;
        }

        public virtual void EnterCam()
        {
            this.gameObject.SetActive(true);
            _virtualCam.m_Priority = 10;
            _virtualCam.m_Lens.OrthographicSize = _originOrthoSize;
        }

        public virtual void ExitCam()
        {
            this.gameObject.SetActive(false);
            _virtualCam.m_Priority = 0;
        }

        public void SetFollowTarget(Transform followTarget)
        {
            _virtualCam.Follow = followTarget;
        }

        public void SetOffset(Vector3 offset)
        {
            var transposer = _virtualCam.GetCinemachineComponent<CinemachineTransposer>();
            var framingTransposer = _virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();

            if (transposer)
            {
                transposer.m_FollowOffset = offset;
            }

            if (framingTransposer)
            {
                framingTransposer.m_TrackedObjectOffset = offset;
            }
        }

        public void SetLookAtTarget(Transform lookAtTarget)
        {
            _virtualCam.LookAt = lookAtTarget;
        }

        public void SetDamping(Vector3 damping)
        {
            var transposer = _virtualCam.GetCinemachineComponent<CinemachineTransposer>();
            var framingTransposer = _virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();

            if (transposer)
            {
                transposer.m_XDamping = damping.x;
                transposer.m_YDamping = damping.y;
                transposer.m_ZDamping = damping.z;
            }

            if (framingTransposer)
            {
                framingTransposer.m_XDamping = damping.x;
                framingTransposer.m_YDamping = damping.y;
                framingTransposer.m_ZDamping = damping.z;
            }
        }

        public void Zoom(float zoomScale, float timer)
        {
            var targetCamSize = _originOrthoSize * zoomScale;
            StartSafeCoroutine("ZoomRoutine", OrthoSizeChangeRoutine(targetCamSize, timer, CameraManager.Instance.ZoomControlCurve));
        }

        public void ShakeCam(float intensity, float time)
        {
           StartSafeCoroutine("ShakeCamRoutine", ShakeSequence(intensity, time));
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
            yield return StartCoroutine(OrthoSizeChangeRoutine(_originOrthoSize + intensity, time / 2f, CameraManager.Instance.CamShakeCurve));
            yield return StartCoroutine(OrthoSizeChangeRoutine(_originOrthoSize, time / 2f, CameraManager.Instance.CamShakeCurve));
        }

        private IEnumerator OrthoSizeChangeRoutine(float targetSize, float timer, AnimationCurve customCurve = null)
        {
            var origin = _virtualCam.m_Lens.OrthographicSize;

            if (Math.Abs(origin - targetSize) < 0.01f)
            {
                yield break;
            }

            var time = 0f;
            var percent = 0f;

            while (percent < 1f)
            {
                time += Time.deltaTime;
                percent = time / timer;

                if (customCurve is not null)
                {
                    percent = customCurve.Evaluate(percent);
                }

                _virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(origin, targetSize, percent);
                
                yield return null;
            }
        }
        
        protected IEnumerator CamRotateRoutine(float rotateValue, float time)
        {
            var currentRot = transform.localRotation;

            var localEulerAngle = transform.localEulerAngles;
            var targetRot = Quaternion.Euler(localEulerAngle.x, rotateValue, localEulerAngle.z);

            var currentTime = 0f;
            while (currentTime < time)
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