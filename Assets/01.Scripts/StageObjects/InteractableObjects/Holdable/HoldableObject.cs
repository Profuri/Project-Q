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

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        if (_holdingHandler is not null)
        {
            SetPosition(_holdingHandler.HoldingPoint);
        }
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        var player = (PlayerUnit)communicator;
        _holdingHandler = player.HoldingHandler;
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
        _holdingHandler = null;
        StopImmediately(true);
        useGravity = true;
    }

    public override void OnPush()
    {
        base.OnPush();
        if (_holdingHandler is not null)
        {
            Detach();
        }
    }
}