using InteractableSystem;
using AxisConvertSystem;

public class HoldableObject : InteractableObject, IHoldable
{
    private ObjectHoldingHandler _holdingHandler;
    
    public override void Awake()
    {
        base.Awake();
        _holdingHandler = null;
    }

    public override void LateUpdateUnit()
    {
        base.LateUpdateUnit();
        if (_holdingHandler is not null)
        {
            useGravity = false;
            StopImmediately(true);
            SetPosition(_holdingHandler.HoldingPoint);
        }
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        var player = (PlayerUnit)communicator;
        player.HoldingHandler.Attach(this);
    }
    
    public void Attach(ObjectHoldingHandler handler)
    {
        _holdingHandler = handler;    
        useGravity = false;
        StopImmediately(true);
    }

    public void Detach()
    {
        if (_holdingHandler is null)
        {
            return;
        }
            
        _holdingHandler = null;
        useGravity = true;
        StopImmediately(true);
    }

    public override void Activate(bool active)
    {
        if (!active)
        {
            Detach();
        }
        
        base.Activate(active);
    }

    public override void OnPush()
    {
        base.OnPush();
        Detach();
    }
}