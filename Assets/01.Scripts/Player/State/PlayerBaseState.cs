public abstract class PlayerBaseState : State
{
    protected bool IsControllingAxis;

    protected PlayerBaseState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
        IsControllingAxis = false;
    }

    protected void JumpHandle()
    {
        Controller.ChangeState(typeof(PlayerJumpState));
    }
    
    protected void AxisControlHandle()
    {
        if (!Player.Converter.Convertable || !Player.CanAxisControl || StageManager.Instance.CurrentStage.IsClear)
        {
            return;
        }

        if (IsControllingAxis)
        {
            Controller.ChangeState(typeof(PlayerIdleState));
        }
        else
        {
            Controller.ChangeState(typeof(PlayerAxisControlState));
        }
    }
}