using System;
using System.Collections.Generic;
using UnityEngine;
using VirtualCam;

public class StageCamController : VirtualCamController
{
    private readonly Dictionary<EAxisType, VirtualCamComponent> _virtualCamDiction =
        new Dictionary<EAxisType, VirtualCamComponent>();

    public override void Init()
    {
        base.Init();
        _virtualCamDiction.Clear();
        foreach (var vCam in _virtualCams)
        {
            var cam = (StageVirtualCam)vCam;
            _virtualCamDiction.Add(cam.AxisType, vCam);
        }   
    }

    public void ChangeStageCamera(EAxisType type, Action callBack = null)
    {
        CurrentSelectedCam = _virtualCamDiction[type];
        SetCurrentCam(callBack);
    }

    public void SetStage(Stage stage)
    {
        var stageTrm = stage.transform;
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