using System;
using System.Collections.Generic;
using UnityEngine;
using VirtualCam;

public class SectionCamController : VirtualCamController
{
    private readonly Dictionary<EAxisType, VirtualCamComponent> _virtualCamDiction =
        new Dictionary<EAxisType, VirtualCamComponent>();

    private VirtualCamComponent _axisControlCam;

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
            _axisControlCam.SetAxisXValue(CurrentSelectedCam.GetAxisXValue());
            _axisControlCam.SetAxisYValue(CurrentSelectedCam.GetAxisYValue());
            
            CurrentSelectedCam = _axisControlCam;
            SetCurrentCam(callBack);
        }
        else
        {
            ChangeCameraAxis(EAxisType.NONE);
        }
        
    }

    public void ChangeCameraAxis(EAxisType type, Action callBack = null)
    {
        CurrentSelectedCam = _virtualCamDiction[type];
        SetCurrentCam(callBack);
    }

    public void SetPlayer(PlayerController player)
    {
        var playerTrm = player.transform;
        
        _axisControlCam.SetFollowTarget(playerTrm);
        _axisControlCam.SetLookAtTarget(playerTrm);

        foreach (EAxisType axis in Enum.GetValues(typeof(EAxisType)))
        {
            _virtualCamDiction[axis].SetFollowTarget(playerTrm);
        }
    }
}