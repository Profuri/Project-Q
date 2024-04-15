using AxisConvertSystem;
using UnityEngine;

public class PlayerJumpState : PlayerOnAirState
{
    private float _enterTime = 0f;
    public bool IsFalling => Player.Rigidbody.velocity.y < 0f;
    public PlayerJumpState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
        _enterTime = 0f;
    }

    public override void EnterState()
    {
        base.EnterState();
        _enterTime = Time.time;

        if (Player.Converter.AxisType == AxisType.Y)
        {
            Controller.ChangeState(typeof(PlayerIdleState));
            return;
        }
        
        Player.SetVelocity(Vector3.up * Player.Data.jumpPower, false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if(Player.IsCoyote) return;
        if (IsFalling)
        {
            Controller.ChangeState(typeof(PlayerFallState));
        }
    }
}
