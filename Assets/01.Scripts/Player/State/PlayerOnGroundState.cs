public class PlayerOnGroundState : PlayerBaseState
{
    public PlayerOnGroundState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        InputManager.Instance.PlayerInputReader.OnJumpEvent += JumpHandle;
        InputManager.Instance.PlayerInputReader.OnAxisControlEvent += AxisControlHandle;
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
        InputManager.Instance.PlayerInputReader.OnJumpEvent -= JumpHandle;
        InputManager.Instance.PlayerInputReader.OnAxisControlEvent -= AxisControlHandle;
    }
}