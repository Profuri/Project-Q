using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcherInEditor : MonoBehaviour
{
    public static CinemachineVirtualCameraBase Camera_X = GameObject.Find("CompressCamera (X)").GetComponent(typeof(CinemachineVirtualCameraBase)) as CinemachineVirtualCameraBase;
    public static CinemachineVirtualCameraBase Camera_Y = GameObject.Find("CompressCamera (Y)").GetComponent(typeof(CinemachineVirtualCameraBase)) as CinemachineVirtualCameraBase;
    public static CinemachineVirtualCameraBase Camera_Z = GameObject.Find("CompressCamera (Z)").GetComponent(typeof(CinemachineVirtualCameraBase)) as CinemachineVirtualCameraBase;
    public static CinemachineVirtualCameraBase Camera_Base = GameObject.Find("PlayerFreelockCamera").GetComponent(typeof(CinemachineVirtualCameraBase)) as CinemachineVirtualCameraBase;

    private Dictionary<KeyCode, CinemachineVirtualCameraBase> _virtualCamDiction = new();

    private CinemachineVirtualCameraBase _currentVCam;

    private const KeyCode SwitchX = KeyCode.X;
    private const KeyCode SwithcY = KeyCode.Y;
    private const KeyCode SwitchZ = KeyCode.Z;
    private const KeyCode Reset = KeyCode.Space;
    
    bool _initialized = false;

    private void Awake()
    {
        if (_initialized) return;

        Debug.Log("B");
        _virtualCamDiction.Add(SwitchX, Camera_X);
        _virtualCamDiction.Add(SwithcY, Camera_Y);
        _virtualCamDiction.Add(SwitchZ, Camera_Z);
        _virtualCamDiction.Add(Reset, Camera_Base);

        _initialized = true;
    }

    private void OnDisable()
    {
        _currentVCam = null;
        _virtualCamDiction.Clear();
        _virtualCamDiction = null;
        _initialized = false;
    }

    public void SwitchHere(KeyCode _inputKey)
    {
        if (_inputKey == SwitchX || _inputKey == SwithcY || _inputKey == SwitchZ || _inputKey == Reset)
        {
            CameraSwitch(_inputKey);
        }
    }

    private void CameraSwitch(KeyCode _inputKey)
    {
        _currentVCam.Priority = 0;
        _currentVCam = _virtualCamDiction[_inputKey];
        _currentVCam.Priority = 10;
    }
}