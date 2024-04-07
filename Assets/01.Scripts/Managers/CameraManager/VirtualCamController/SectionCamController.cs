using System;
using System.Collections.Generic;
using AxisConvertSystem;
using VirtualCam;
using UnityEngine;


public class SectionCamController : VirtualCamController
{
    private Vector3 _originPos;
    private readonly Dictionary<AxisType, VirtualCamComponent> _virtualCamDiction =
        new Dictionary<AxisType, VirtualCamComponent>();

    private VirtualCamComponent _axisControlCam;
    [SerializeField] private VirtualCamComponent _normalCam;

    public override void Init()
    {
        base.Init();

        if(_normalCam == null)
        {
            Debug.LogError($"Please setting normal cam in inspector !!");
            return;
        }

        _virtualCamDiction.Clear();
        foreach (var vCam in _virtualCams)
        {
            var cam = (SectionVirtualCam)vCam;

            if (cam.IsControlCam)
            {
                _axisControlCam = cam;
            }
            else
            {
                _virtualCamDiction.Add(cam.AxisType, vCam);
            }
        }
        _originPos = _normalCam.transform.position;
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
            ChangeCameraAxis(AxisType.None);
        }
        
    }

    public void ChangeCameraAxis(AxisType type, Action callBack = null)
    {
        CurrentSelectedCam = _virtualCamDiction[type];
        SetCurrentCam(callBack);
    }

    public void SetPlayer(PlayerUnit player)
    {
        var playerTrm = player?.transform;
        
        _axisControlCam.SetFollowTarget(playerTrm);
        _axisControlCam.SetLookAtTarget(playerTrm);

        foreach (AxisType axis in Enum.GetValues(typeof(AxisType)))
        {
            _virtualCamDiction[axis].SetFollowTarget(playerTrm);
        }
    }

    public override void ResetCamera()
    {
        _normalCam.enabled = false;
        _normalCam.transform.position = _originPos;
        _normalCam.enabled = true;
        ChangeCameraAxis(AxisType.None);
    }
}