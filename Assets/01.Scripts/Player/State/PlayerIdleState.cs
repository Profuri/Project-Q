public class PlayerIdleState : PlayerOnGroundState
{
    public PlayerIdleState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Player.StopImmediately(false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        var movementInput = Player.InputReader.movementInput;
        if (movementInput.sqrMagnitude > 0.05f)
        {
            Controller.ChangeState(typeof(PlayerMovementState));
        }
    }
}
