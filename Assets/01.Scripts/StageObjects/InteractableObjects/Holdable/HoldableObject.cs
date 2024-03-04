using InteractableSystem;
using AxisConvertSystem;

public class HoldableObject : InteractableObject
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        var player = (PlayerUnit)communicator;
        player.HoldingHandler.Attach(this);
    }
}