public abstract class PlayerBaseState : State
{
    protected bool IsControllingAxis;

    public PlayerBaseState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
        IsControllingAxis = false;
    }

    protected void JumpHandle()
    {
        Controller.ChangeState(typeof(PlayerJumpState));
    }
    
    protected void AxisControlHandle()
    {
        if (!Player.Converter.Convertable || !Player.CanAxisControl)
        {
            return;
        }

        if (IsControllingAxis)
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