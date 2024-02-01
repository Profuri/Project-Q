using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractableSystem;
using System;
using AxisConvertSystem;

public class ToggleObject : InteractableObject
{
    public bool IsToggle => _isToggle;
    private bool _isToggle = false;
    private event Action _ToggleEvent;
    
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        _isToggle = interactValue;
        gameObject.SetActive(_isToggle);
        InterEnd = true;
    }
}
