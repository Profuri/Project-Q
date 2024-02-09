public class PlayerIdleState : State
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
        var movementInput = Player.InputReader.movementInput;
        if (movementInput.sqrMagnitude > 0f)
        {
            Controller.ChangeState(typeof(PlayerMovementState));
        }
    }
}
