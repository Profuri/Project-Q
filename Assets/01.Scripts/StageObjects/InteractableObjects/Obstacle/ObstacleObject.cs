using System;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class ObstacleObject : InteractableObject
{
    [SerializeField] private LayerMask _damageableMask;

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        OnInteraction(null, false);
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        base.ApplyUnitInfo(axis);
        OnInteraction(null, false);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if(!activeUnit)
        {
            return;
        }

        var pos = Collider.bounds.center;
        var checkSize = Collider.bounds.size;
        var rotation = transform.rotation;

        var cols = new Collider[10];
        var size = Physics.OverlapBoxNonAlloc(pos, checkSize / 2, cols, rotation, _damageableMask);

        for (var i = 0; i < size; i++)
        {
            if (cols[i].TryGetComponent<ObjectUnit>(out var unit))
            {
                if (unit is InteractableObject interactable)
                {
                    if (interactable.InteractType == EInteractType.AFFECTED_OTHER &&
                        interactable.Attribute.HasFlag(EInteractableAttribute.DAMAGED_BY_THORN))
                    {
                        interactable.OnInteraction(communicator, interactValue);
                        return;
                    }
                }
                
                if (!unit.staticUnit || (unit.Collider.excludeLayers & (1 << gameObject.layer)) == 0)
                {
                    unit.ReloadUnit();
                }
            }
        }
    }
}