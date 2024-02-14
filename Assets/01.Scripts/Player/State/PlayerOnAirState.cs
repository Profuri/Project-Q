using AxisConvertSystem;
using UnityEngine;

public class PlayerOnAirState : PlayerBaseState
{
    public PlayerOnAirState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Player.InputReader.OnAxisControlEvent += AxisControlHandle;
    }

    public override void UpdateState()
    {
        if (Player.OnGround)
        {
            Controller.ChangeState(typeof(PlayerIdleState));
            return;
        }
        
        var movementInput = Player.InputReader.movementInput;
        
        var dir = Quaternion.Euler(0, CameraManager.Instance.ActiveVCam.transform.eulerAngles.y, 0) * movementInput;
        if (Player.Converter.AxisType != AxisType.None)
        {
            dir.SetAxisElement(Player.Converter.AxisType, 0);
        }
        
        if (dir.sqrMagnitude > 0.05f)
        {
            Player.Rotate(Quaternion.LookRotation(dir), Player.Converter.AxisType is AxisType.None or AxisType.Y ? Player.Data.rotationSpeed : 1f);
        }
        Player.SetVelocity(dir * Player.Data.walkSpeed);
    }

    public override void ExitState()
    {
        base.ExitState();
        Player.InputReader.OnAxisControlEvent -= AxisControlHandle;
    }
}