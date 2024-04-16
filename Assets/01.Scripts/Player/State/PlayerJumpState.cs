using AxisConvertSystem;
using UnityEngine;

public class PlayerJumpState : PlayerOnAirState
{
    public bool IsFalling => Player.Rigidbody.velocity.y < 0f;
    private StateController _controller;
    public PlayerJumpState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
        _controller = controller;
    }
    public override void EnterState()
    {
        InputManager.Instance.PlayerInputReader.OnJumpEvent -= JumpHandle;


        if (!Player.CanJump) return;

        base.EnterState();


        if (Player.Converter.AxisType == AxisType.Y)
        {
            Controller.ChangeState(typeof(PlayerIdleState));
            return;
        }
        Player.SetVelocity(Vector3.up * Player.Data.jumpPower, false);
        Player.ResetCoyoteTime();
    }

    public override void UpdateState()
    {
        var movementInput = InputManager.Instance.PlayerInputReader.movementInput;
        
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

        if (IsFalling)
        {
            //Debug.Break();
            Player.ResetCoyoteTime();
            Controller.ChangeState(typeof(PlayerFallState));
        }
    }
}
