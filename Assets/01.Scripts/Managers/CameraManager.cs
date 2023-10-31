using System.Collections.Generic;
using Cinemachine;
using ManagingSystem;
using UnityEngine;
using static Core.Define;

public class CameraManager : BaseManager<CameraManager>
{
    [SerializeField] private List<CinemachineVirtualCameraBase> _virtualCams = new List<CinemachineVirtualCameraBase>();

    private CinemachineVirtualCameraBase _currentVCam;

    public override void Awake()
    {
        base.Awake();
        GameManager.Instance.OnAxisTypeChangeEvent += ChangeCamera;
    }

    public override void StartManager()
    {
        _currentVCam = null;
        foreach (var vCam in _virtualCams)
        {
            vCam.m_Priority = 0;
        }
        ChangeCamera(EAxisType.NONE);
    }

    public override void UpdateManager()
    {
        // Do Nothing
    }

    private void ChangeCamera(EAxisType type)
    {
        if (_currentVCam != null)
        {
            _currentVCam.m_Priority = 0;
            _currentVCam = null;
        }

        _currentVCam = _virtualCams[(int)type];
        _currentVCam.m_Priority = 10;
    }
}