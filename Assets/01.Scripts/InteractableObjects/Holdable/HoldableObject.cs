using System;
using InteractableSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldableObject : InteractableObject
{
    private Rigidbody _rigid;
    private bool _isHeld;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
        _isHeld = false;
    }

    public override void OnInteraction(PlayerController player, bool interactValue)
    {
        _isHeld = !_isHeld;
        _rigid.useGravity = !_isHeld;
        
        var holdingModule = player.GetModule<PlayerObjectHoldingModule>();

        if (_isHeld)
        {
            holdingModule.AttachObject(this);
        }
        else
        {
            holdingModule.DetachObject();
        }
    }
}