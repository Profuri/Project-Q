using System;
using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class ThornObject : InteractableObject
{
    [SerializeField] private LayerMask _damageableMask;

    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }
    
    private void Update()
    {
        OnInteraction(null, false);
    }

    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue)
    {
        var pos = transform.position + _boxCollider.center;
        var checkSize = _boxCollider.size;
        var rotation = transform.rotation;

        var cols = new Collider[10];
        var size = Physics.OverlapBoxNonAlloc(pos, checkSize / 2, cols, rotation, _damageableMask);

        for (var i = 0; i < size; i++)
        {
            if (cols[i].TryGetComponent<PlayableObjectUnit>(out var playerUnit))
            {
                playerUnit.ReloadObject();
            }
            else if (cols[i].TryGetComponent<InteractableObject>(out var interactable))
            {
                if (interactable.InteractType == EInteractType.AFFECTED_OTHER &&
                    interactable.Attribute.HasFlag(EInteractableAttribute.DAMAGED_BY_THORN))
                {
                    interactable.OnInteraction(communicator, interactValue);
                }
            }
        }
    }
}