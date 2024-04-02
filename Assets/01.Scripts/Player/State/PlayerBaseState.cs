using UnityEngine;
public abstract class PlayerBaseState : State
{
    protected static bool _toggle = false;
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

        if (_toggle)
        {
            Player.Converter.ShowClimbableEffect();
            Controller.ChangeState(typeof(PlayerAxisControlState));
        }
        else
        {
            Player.Converter.UnShowClimbableEffect();
            Controller.ChangeState(typeof(PlayerIdleState));
        }
        _toggle = !_toggle;
    }
}