using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;
using VirtualCam;

public class PlayerAxisControlState : PlayerBaseState
{
    private AxisType _controllingAxis;
    
    public PlayerAxisControlState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        IsControllingAxis = true;
        InputManagerHelper.OnControllingAxis();

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
            InputManager.Instance.PlayerInputReader.OnClickEvent += SelectAxisHandle;
            
            VolumeManager.Instance.SetAxisControlVolume(true, 0.2f);
            ((SectionCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(true);
        }
        else
        {
            SelectAxisHandle();
        }
    }

    public override void UpdateState()
    {
        CalcCurrentControlAxis();
        LightManager.Instance.SetAxisLight(_controllingAxis);
    }

    public override void ExitState()
    {
        base.ExitState();
        IsControllingAxis = false;
        InputManagerHelper.OnCancelingAxis();

        InputManager.Instance.PlayerInputReader.OnAxisControlEvent -= AxisControlHandle;
        InputManager.Instance.PlayerInputReader.OnClickEvent -= SelectAxisHandle;
        
        VolumeManager.Instance.SetAxisControlVolume(false, 0.2f);
        LightManager.Instance.SetAxisLight(AxisType.None);

        var currentCamController = CameraManager.Instance.CurrentCamController as SectionCamController;
        if (currentCamController && currentCamController.CurrentSelectedCam is SectionVirtualCam { AxisType: AxisType.None})
        {
            currentCamController.SetAxisControlCam(false);
        }
        
        InputManager.Instance.SetEnableInputAll(true);
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
        InputManager.Instance.SetEnableInputWithout(EInputCategory.Escape,false);
        Player.Converter.ConvertDimension(_controllingAxis, InputManagerHelper.OnCancelingAxis);
    }
}