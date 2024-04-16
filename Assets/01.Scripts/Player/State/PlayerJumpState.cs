using AxisConvertSystem;
using UnityEngine;

public class PlayerJumpState : PlayerOnAirState
{
    public bool IsFalling => Player.Rigidbody.velocity.y < 0f;
    public PlayerJumpState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
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
        base.UpdateState();

        if (IsFalling)
        {
            Controller.ChangeState(typeof(PlayerFallState));
        }
    }
}
