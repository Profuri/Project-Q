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
        bool isClear = StageManager.Instance.CurrentStage.IsClear;
        if (!Player.Converter.Convertable || !Player.CanAxisControl || isClear)
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