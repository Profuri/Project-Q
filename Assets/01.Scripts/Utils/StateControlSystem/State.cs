using UnityEngine;

public abstract class State
{
    protected StateController Controller { get; private set; }
    protected PlayerUnit Player { get; private set; }

    private readonly bool _useAnim;
    
    protected bool AnimationTriggerCalled;
    private readonly int _animationHash;

    public State(StateController controller, bool useAnim = false, string animationKey = "")
    {
        Controller = controller;
        Player = Controller.Owner;
        _useAnim = useAnim;

        if (_useAnim)
        {
            _animationHash = Animator.StringToHash(animationKey);
        }
    }
    
    public virtual void EnterState()
    {
        if (_useAnim)
        {
            AnimationTriggerCalled = false;
            Player.Animator.SetBool(_animationHash, true);
        }
    }

    public abstract void UpdateState();
    
    public virtual void ExitState()
    {
        if (_useAnim)
        {
            Player.Animator.SetBool(_animationHash, false);
        }
    }

    public virtual void AnimationTrigger(string triggerKey)
    {
        if (_useAnim)
        {
            AnimationTriggerCalled = true;
        }
    }
}