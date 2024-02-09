using AxisConvertSystem;
using UnityEngine;

public abstract class State
{
    protected StateController Controller { get; private set; }
    protected ObjectUnit Owner { get; private set; }

    protected bool _animationTriggerCalled;

    private readonly int _animationHash;
    
    public State(StateController controller, string animationParameter)
    {
        Controller = controller;
        Owner = Controller.Owner;
        _animationHash = Animator.StringToHash(animationParameter);
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}