using System;
using System.Collections.Generic;
using AxisConvertSystem;
using VirtualCam;
using UnityEngine;


public class SectionCamController : VirtualCamController
{
    private Vector3 _originPos;
    private Quaternion _originRot;
    
    private readonly Dictionary<AxisType, VirtualCamComponent> _virtualCamDiction =
        new Dictionary<AxisType, VirtualCamComponent>();

    private Dictionary<VirtualCamComponent,Vector3> _virtualCamPosDictionary = new Dictionary<VirtualCamComponent, Vector3>();

    private VirtualCamComponent _axisControlCam;

    [SerializeField] private float _changeAxisConvertCamDelay = 0.25f;

    public override void Init()
    {
        base.Init();

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

        foreach(var kvp in _virtualCamDiction)
        {
            Vector3 originPos = kvp.Value.transform.position;
            _virtualCamPosDictionary.Add(kvp.Value,originPos);
        }
        _originPos = _virtualCamDiction[AxisType.None].transform.position;
        _originRot = _virtualCamDiction[AxisType.None].transform.rotation;
    }

    public void SetAxisControlCam(bool value, Action callBack = null)
    {
        if (value)
        {
            CurrentSelectedCam.RotateCam(CameraManager.Instance.InitRotateValue, _changeAxisConvertCamDelay, () =>
            {
                _axisControlCam.SetAxisXValue(-45f);
                _axisControlCam.SetAxisYValue(45f);
                CurrentSelectedCam = _axisControlCam;
                SetCurrentCam(callBack);
            });
            // CameraManager.Instance.LastRotateValue = CameraManager.Instance.InitRotateValue;
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

        foreach(var kvp in _virtualCamDiction)
        {
            _virtualCamDiction[kvp.Key].enabled = false;
            _virtualCamDiction[kvp.Key].transform.position = _originPos;
            _virtualCamDiction[kvp.Key].enabled = true;
        }

        _virtualCamDiction[AxisType.None].transform.rotation = _originRot;
        ChangeCameraAxis(AxisType.None);
    }
}