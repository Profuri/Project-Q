using System.Collections.Generic;
using AxisConvertSystem;

public class PlayerAxisControlState : PlayerBaseState
{
    private AxisType _controllingAxis;
    private SoundEffectPlayer _soundEffectPlayer;
    
    public PlayerAxisControlState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
        _soundEffectPlayer = new SoundEffectPlayer(controller.Owner);
    }

    public override void EnterState()
    {
        base.EnterState();

        SoundManager.Instance.PlaySFX("AxisControl",true,);

        _isControllingAxis = true;

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
        InputManagerHelper.OnControllingAxis();
    }

    public override void UpdateState()
    {
        CalcCurrentControlAxis();
        LightManager.Instance.SetAxisLight(_controllingAxis);
    }

    public override void ExitState()
    {
        base.ExitState();

        _isControllingAxis = false;
        InputManager.Instance.PlayerInputReader.OnAxisControlEvent -= AxisControlHandle;
        InputManager.Instance.PlayerInputReader.OnClickEvent -= SelectAxisHandle;
        
        VolumeManager.Instance.SetAxisControlVolume(false, 0.2f);
        LightManager.Instance.SetAxisLight(AxisType.None);
        ((SectionCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(false);
        
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
        
        // block input
        InputManager.Instance.SetEnableInputAll(false);
        Player.Converter.ConvertDimension(_controllingAxis,() => 
            InputManagerHelper.OnCancelingAxis());

        _isControllingAxis = false;
    }
}