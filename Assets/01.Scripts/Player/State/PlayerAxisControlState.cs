using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AxisConvertSystem;
using DG.Tweening;
using UnityEngine;

public class PlayerAxisControlState : PlayerBaseState
{
    private AxisType _controllingAxis;
    
    public PlayerAxisControlState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        
        if (!Player.Converter.Convertable)
        {
            Controller.ChangeState(typeof(PlayerIdleState));
            return;
        }

        _controllingAxis = AxisType.None;

        if (Player.Converter.AxisType == AxisType.None)
        {
            Player.InputReader.OnAxisControlEvent += AxisControlHandle;
            Player.InputReader.OnClickEvent += SelectAxisHandle;
            
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
        Player.InputReader.OnAxisControlEvent -= AxisControlHandle;
        Player.InputReader.OnClickEvent -= SelectAxisHandle;
        VolumeManager.Instance.SetAxisControlVolume(false, 0.2f);
        LightManager.Instance.SetAxisLight(AxisType.None);
        ((SectionCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(false);
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
        
        // block input

        Player.Converter.ConvertDimension(_controllingAxis);
    }
}