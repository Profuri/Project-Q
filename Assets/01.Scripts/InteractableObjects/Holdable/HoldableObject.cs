using System;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldableObject : InteractableObject
{
    private Rigidbody _rigid;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
        IsInteract = false;
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        IsInteract = !IsInteract;
        _rigid.useGravity = !IsInteract;
        _rigid.freezeRotation = IsInteract;

        var player = (PlayerUnit)communicator;
        // var holdingModule = player.GetModule<PlayerObjectHoldingModule>();

        if (IsInteract)
        {
            // holdingModule.AttachObject(this);
        }
        else
        {
            // holdingModule.DetachObject();
        }
    }
}