using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VirtualCam;

public class StageCamController : VirtualCamController
{
    private readonly Dictionary<EAxisType, VirtualCamComponent> _virtualCamDiction =
        new Dictionary<EAxisType, VirtualCamComponent>();

    private VirtualCamComponent _axisControlCam;
    private VirtualCamComponent _prevStageCam;

    public override void Init()
    {
        base.Init();
        _virtualCamDiction.Clear();
        foreach (var vCam in _virtualCams)
        {
            var cam = (StageVirtualCam)vCam;

            if (cam.IsControlCam)
            {
                _axisControlCam = cam;
            }
            else
            {
                _virtualCamDiction.Add(cam.AxisType, vCam);
            }
        }   
    }

    public void SetAxisControlCam(bool value, Action callBack = null)
    {
        if (value)
        {
            _prevStageCam = CurrentSelectedCam;
            CurrentSelectedCam = _axisControlCam;
        }
        else
        {
            CurrentSelectedCam = _prevStageCam;
        }
        
        SetCurrentCam(callBack);
    }

    public void ChangeStageCamera(EAxisType type, Action callBack = null)
    {
        CurrentSelectedCam = _virtualCamDiction[type];
        SetCurrentCam(callBack);
    }

    public void SetStage(Stage stage)
    {
        var stageTrm = stage.transform;
        
        _axisControlCam.SetFollowTarget(stageTrm);
        _axisControlCam.SetLookAtTarget(stageTrm);
        
        foreach (EAxisType axis in Enum.GetValues(typeof(EAxisType)))
        {
            _virtualCamDiction[axis].SetFollowTarget(stageTrm);
            if (axis == EAxisType.NONE)
            {
                _virtualCamDiction[axis].SetLookAtTarget(stageTrm);
            }
        }
    }
}