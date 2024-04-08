using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;


public class HoldableObject : InteractableObject, IHoldable
{
    public ObjectUnit GetObjectUnit() => this;

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        var player = (PlayerUnit)communicator;
        player.HoldingHandler.Attach(this);
    }
}