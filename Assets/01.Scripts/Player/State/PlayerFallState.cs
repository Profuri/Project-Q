public class PlayerFallState : PlayerOnAirState
{
    public PlayerFallState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        InputManager.Instance.PlayerInputReader.OnJumpEvent += JumpHandle;

    }

    public override void ExitState()
    {
        base.ExitState();
        InputManager.Instance.PlayerInputReader.OnJumpEvent -= JumpHandle;
    }
}