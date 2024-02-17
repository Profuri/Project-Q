public class PlayerOnGroundState : PlayerBaseState
{
    public PlayerOnGroundState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Player.InputReader.OnJumpEvent += JumpHandle;
        Player.InputReader.OnAxisControlEvent += AxisControlHandle;
    }

    public override void UpdateState()
    {
        if (!Player.OnGround)
        {
            Controller.ChangeState(typeof(PlayerFallState));
            return;
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        Player.InputReader.OnJumpEvent -= JumpHandle;
        Player.InputReader.OnAxisControlEvent -= AxisControlHandle;
    }
}