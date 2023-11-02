using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ManagingSystem;
using UnityEngine;
using static Core.Define;

public class CameraManager : BaseManager<CameraManager>
{
    [SerializeField] private List<CinemachineVirtualCameraBase> _virtualCams = new List<CinemachineVirtualCameraBase>();

    private CinemachineVirtualCameraBase _currentVCam;

    public override void StartManager()
    {
        _currentVCam = null;
        foreach (var vCam in _virtualCams)
        {
            vCam.m_Priority = 0;
        }
        ChangeCamera(EAxisType.NONE, null);
    }

    public override void UpdateManager()
    {
        // Do Nothing
    }

    public void ChangeCamera(EAxisType type, Action CallBack = null)
    {
        StartCoroutine(ChangeCamRoutine(type, 1f, CallBack));
    }

    private IEnumerator ChangeCamRoutine(EAxisType type, float time, Action CallBack)
    {
        if (type == EAxisType.NONE)
        {
            CallBack?.Invoke();

            if (_currentVCam != null)
            {
                _currentVCam.m_Priority = 0;
                _currentVCam = null;
            }
            
            _currentVCam = _virtualCams[(int)type];
            _currentVCam.m_Priority = 10;
        }
        else
        {
            if (_currentVCam != null)
            {
                _currentVCam.m_Priority = 0;
                _currentVCam = null;
            }
            
            _currentVCam = _virtualCams[(int)type];
            _currentVCam.m_Priority = 10;

            yield return new WaitForSeconds(time);
            
            CallBack?.Invoke();
        }
    }

    public void SetOrthographic(bool value)
    {
        MainCam.orthographic = value;
    }

    public void ShakeCam()
    {
        if (_currentVCam.TryGetComponent<CinemachineImpulseSource>(out var source))
        {
            source.GenerateImpulse();
        }
    }
}