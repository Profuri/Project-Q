public class PlayerMovementState : PlayerOnGroundState
{
    public PlayerMovementState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }
    
    public override void UpdateState()
    {
        base.UpdateState();

        var movementInput = Player.InputReader.movementInput;
        if (movementInput.sqrMagnitude < 0.05f)
        {
            Controller.ChangeState(typeof(PlayerIdleState));
            return;
        }
        
        Player.SetVelocity(movementInput * Player.Data.walkSpeed);
    }
}