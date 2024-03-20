public abstract class PlayerBaseState : State
{
    public PlayerBaseState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    protected void JumpHandle()
    {
        Controller.ChangeState(typeof(PlayerJumpState));
    }

    protected void AxisControlHandle(bool toggle)
    {
        if (!Player.Converter.Convertable)
        {
            return;
        }
        
        if (toggle)
        {
            Player.Converter.ShowClimbableEffect();
            Controller.ChangeState(typeof(PlayerAxisControlState));
        }
        else
        {
            Player.Converter.UnShowClimbableEffect();
            Controller.ChangeState(typeof(PlayerIdleState));
        }
    }
}