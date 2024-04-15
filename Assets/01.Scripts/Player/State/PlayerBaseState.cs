using UnityEngine;
public abstract class PlayerBaseState : State
{
    protected static bool _isControllingAxis = false;

    public PlayerBaseState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    protected void JumpHandle()
    {
        Controller.ChangeState(typeof(PlayerJumpState));
    }
    protected void AxisControlHandle()
    {
        if (!Player.Converter.Convertable)
        {
            return;
        }

        if (_isControllingAxis)
        {
            Player.Converter.UnShowClimbableEffect();
            Controller.ChangeState(typeof(PlayerIdleState));
        }
        else
        {
            Player.Converter.ShowClimbableEffect();
            Controller.ChangeState(typeof(PlayerAxisControlState));
        }
    }
}