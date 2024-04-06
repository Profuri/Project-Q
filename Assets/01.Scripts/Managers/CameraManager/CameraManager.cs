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

    [Header("Offset Setting")] 
    [SerializeField] private AnimationCurve _camOffsetCurve;
    public AnimationCurve CamOffsetCurve => _camShakeCurve;
    [SerializeField] private float _offsetAmount;
    public float OffsetAmount => _offsetAmount;
    [SerializeField] private float _turnBackOffsetDelay;
    [SerializeField] private float _offsetApplyDelay;

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
                Debug.LogError("[CameraManager] please attach all type of cam controller this object");
                return;
            }
            
            camController.Init();
            _vCamControllers.Add(camType, camController);
        }
    }

    public void ApplyInitOffset()
    {
        ActiveVCam.ApplyOffset(Vector3.zero, _turnBackOffsetDelay);
    }

    public void ApplyCamOffset(Vector3 dir)
    {
        var adjustDir = Quaternion.Euler(0, ActiveVCam.transform.eulerAngles.y, 0) * dir;
        var offset = adjustDir * _offsetAmount;
        ActiveVCam.ApplyOffset(offset, _offsetApplyDelay);
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

    public void FixedCameraRectWithResolution(int width, int height)
    {
        var viewportRect = new Rect(0, 0, 1, 1);

        var screenAspectRatio = (float)width / height;
        var targetAspectRatio = (float)16 / 9;

        if (screenAspectRatio < targetAspectRatio)
        {
            viewportRect.height = screenAspectRatio / targetAspectRatio;
            viewportRect.y = (1f - viewportRect.height) / 2f;
        }
        else if(screenAspectRatio > targetAspectRatio)
        {
            viewportRect.width = targetAspectRatio / screenAspectRatio;
            viewportRect.x = (1f - viewportRect.width) / 2f;
        }

        MainCam.rect = viewportRect;
    }
}