using System;
using System.Collections;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;
using VirtualCam;

public class CameraManager : BaseManager<CameraManager>
{
    private Dictionary<VirtualCamType, VirtualCamController> _vCamControllers;
    public VirtualCamController CurrentCamController { get; private set; }
    
    public VirtualCamComponent ActiveVCam { get; private set; }
    public Camera MainCam { get; private set; }

    [Header("Transition Setting")]
    [SerializeField] private AnimationCurve _camShakeCurve;
    public AnimationCurve CamShakeCurve => _camShakeCurve;
    [SerializeField] private float _transitionTime;

    public override void StartManager()
    {
        _vCamControllers = new Dictionary<VirtualCamType, VirtualCamController>();
        CurrentCamController = null;
        ActiveVCam = null;
        MainCam = Camera.main;

        foreach (VirtualCamType camType in Enum.GetValues(typeof(VirtualCamType)))
        {
            if (GetComponent($"{camType.ToString().ToUpperFirstChar()}CamController") is not VirtualCamController camController)
            {
                Debug.LogError("please attack all type of cam controller this object");
                return;
            }
            
            camController.Init();
            _vCamControllers.Add(camType, camController);
        }
    }

    public override void UpdateManager()
    {
        ActiveVCam.UpdateCam();
    }

    public void ChangeVCamController(VirtualCamType type)
    {
        CurrentCamController = _vCamControllers[type];
    }

    public void ChangeCamera(VirtualCamComponent virtualCam, Action callBack)
    {
        StartCoroutine(ChangeCamRoutine(virtualCam, _transitionTime, callBack));
    }

    private IEnumerator ChangeCamRoutine(VirtualCamComponent virtualCam, float time, Action callBack)
    {
        if (ActiveVCam is not null)
        {
            ActiveVCam.ExitCam();
            ActiveVCam = null;
        }

        ActiveVCam = virtualCam;
        ActiveVCam.EnterCam();

        yield return new WaitForSeconds(time);
            
        callBack?.Invoke();
    }

    public void ShakeCam(float intensity, float time)
    {
        ActiveVCam.ShakeCam(intensity, time);
    }
}