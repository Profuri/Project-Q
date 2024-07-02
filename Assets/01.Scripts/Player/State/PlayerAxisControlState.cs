using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;
using UnityEngine.Windows;
using System;
using VirtualCam;

public class PlayerAxisControlState : PlayerBaseState
{
    private AxisType _controllingAxis;
    public event Action<AxisType> axisControlingEvent;
    public PlayerAxisControlState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
        
    }

    public override void EnterState()
    {
        base.EnterState();
        Player.SetActiveAnimation(false);
        
        IsControllingAxis = true;
        InputManager.Instance.SetEnableInputWithout(new EInputCategory[] { EInputCategory.AxisControl }, true);

        Player.StopImmediately(true);

        if (!Player.Converter.Convertable)
        {
            Controller.ChangeState(typeof(PlayerIdleState));
            return;
        }

        _controllingAxis = AxisType.None;

        if (Player.Converter.AxisType == AxisType.None)
        {
            InputManager.Instance.PlayerInputReader.OnAxisControlEvent += AxisControlHandle;
            InputManager.Instance.PlayerInputReader.OnAxisConvertEvent += SelectAxisHandle;
            
            VolumeManager.Instance.SetVolume(VolumeType.AxisControlling, 0.2f);
            ((SectionCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(true);
        }
        else
        {
            SelectAxisHandle();
        }
    }

    public override void UpdateState()
    {
        Player.StopImmediately(false);
        CalcCurrentControlAxis();

        Player.OnControllingAxisEvent?.Invoke(_controllingAxis);

        LightManager.Instance.SetAxisLight(_controllingAxis);
    }

    public override void ExitState()
    {
        base.ExitState();
        Player.SetActiveAnimation(true);
        
        IsControllingAxis = false;

        InputManager.Instance.PlayerInputReader.OnAxisControlEvent -= AxisControlHandle;
        InputManager.Instance.PlayerInputReader.OnAxisConvertEvent -= SelectAxisHandle;
        
        VolumeManager.Instance.SetVolume(VolumeType.Default, 0.2f);
        LightManager.Instance.SetAxisLight(AxisType.None);

        var currentCamController = CameraManager.Instance.CurrentCamController as SectionCamController;
        if (currentCamController && currentCamController.CurrentSelectedCam is SectionVirtualCam { AxisType: AxisType.None})
        {
            currentCamController.SetAxisControlCam(false);
        }        
    }
    
    private void CalcCurrentControlAxis()
    {
        var vCam = CameraManager.Instance.ActiveVCam;
        var camYValue = vCam.GetAxisYValue();
        var camXValue = vCam.GetAxisXValue();

        if (camYValue >= 45f)
        {
            _controllingAxis = AxisType.Y;
        }
        else 
        {
            if (camXValue >= -45f)
            {
                _controllingAxis = AxisType.Z;
            }
            else if (camXValue >= -90f)
            {
                _controllingAxis = AxisType.X;
            }
        } 
    }

    private void SelectAxisHandle()
    {
        Controller.ChangeState(typeof(PlayerIdleState));
        
        Player.Converter.ConvertDimension(_controllingAxis, ()=> InputManager.Instance.SetEnableInputAll(true));
    }
}