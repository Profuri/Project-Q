using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;
using UnityEditor.Tilemaps;

public class HoldableObject : InteractableObject, IHoldable
{
    public ObjectUnit GetObjectUnit() => this;

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        base.OnInteraction(communicator, interactValue, param);
        var player = (PlayerUnit)communicator;
        player.HoldingHandler.Attach(this);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }

}