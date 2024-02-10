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
        var dir = Quaternion.Euler(0, -45, 0) * movementInput;
        Player.SetVelocity(dir * Player.Data.walkSpeed);
    }

    public override void ExitState()
    {
        base.ExitState();
        Player.InputReader.OnAxisControlEvent -= AxisControlHandle;
    }
}