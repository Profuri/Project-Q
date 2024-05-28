using UnityEngine;

public class PlayerIdleState : PlayerOnGroundState
{
    public PlayerIdleState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Player.CanAxisControl = true;
        Player.StopImmediately(false);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        Player.StopImmediately(false);

        var movementInput = InputManager.Instance.PlayerInputReader.movementInput;
        if (movementInput.sqrMagnitude > 0.05f)
        {
            Controller.ChangeState(typeof(PlayerMovementState));
        }
    }
}
