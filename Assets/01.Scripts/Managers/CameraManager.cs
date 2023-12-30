using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ManagingSystem;
using UnityEngine;
using VirtualCam;
using static Core.Define;

public class CameraManager : BaseManager<CameraManager>
{
    [SerializeField] private List<VirtualCamComponent> _virtualCams = new();
    private Dictionary<EAxisType, VirtualCamComponent> _virtualCamDiction = new();
    
    private VirtualCamComponent _currentVCam;
    private Camera _mainCam;
    public Camera MainCam => _mainCam;
    public Transform CameraContainerTrm => _mainCam.transform.parent;
    public Vector3 CameraDiff => _cameraDiff;
    private Vector3 _cameraDiff;
    public override void StartManager()
    {
        _mainCam = Camera.main;
        _currentVCam = null;
        _cameraDiff = _mainCam.transform.parent.transform.position;

        foreach (var vCam in _virtualCams)
        {
            vCam.ExitCam();
            _virtualCamDiction.Add(vCam.Type, vCam);
        }

        ChangeCamera(EAxisType.NONE);
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
        if (_currentVCam != null)
        {
            _currentVCam.ExitCam();
            _currentVCam = null;
        }
            
        _currentVCam = _virtualCamDiction[type];
        _currentVCam.EnterCam();

        yield return new WaitForSeconds(time);
            
        CallBack?.Invoke();
    }

    public void ShakeCam(float intensity, float time)
    {
        _currentVCam.ShakeCam(intensity, time);
    }

    public void SetFreeLookFollowAndLookAt(Transform target)
    {
        var freeLookCam = _virtualCamDiction[EAxisType.NONE].VirtualCam;
        freeLookCam.Follow = target;
        freeLookCam.LookAt = target;
    }
}