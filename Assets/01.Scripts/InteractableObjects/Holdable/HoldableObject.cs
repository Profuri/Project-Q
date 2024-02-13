using System;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class HoldableObject : InteractableObject
{
    private Rigidbody _rigid;

    public override void Awake()
    {
        base.Awake();
        _rigid = GetComponent<Rigidbody>();
        IsInteract = false;
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        IsInteract = !IsInteract;
        _rigid.useGravity = !IsInteract;
        _rigid.freezeRotation = IsInteract;

        var player = (PlayerUnit)communicator;

        if (IsInteract)
        {
            player.HoldingHandler.Attach(this);
        }
        else
        {
            player.HoldingHandler.Detach();
        }
    }
}