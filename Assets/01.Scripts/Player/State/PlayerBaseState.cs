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
        if (toggle)
        {
            Controller.ChangeState(typeof(PlayerAxisControlState));
        }
        else
        {
            Controller.ChangeState(typeof(PlayerIdleState));
        }
    }
}