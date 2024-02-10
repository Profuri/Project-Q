using UnityEngine;

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

        var dir = Quaternion.Euler(0, -45, 0) * movementInput;
        Player.Rotate(Quaternion.LookRotation(dir));
        Player.SetVelocity(dir * Player.Data.walkSpeed);
    }
}