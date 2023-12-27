using InteractableSystem;
using StageStructureConvertSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationObject : InteractableObject
{
    private Outline _outLine;
    
    private void Awake()
    {
        IsInteract = false;
        _outLine = GetComponent<Outline>();
    }

    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        IsInteract = !IsInteract;

        var player = ((PlayableObjectUnit)communicator).PlayerController;
        var rotateModule = player.GetModule<PlayerRotateOtherObjectModule>();

        if(IsInteract)
        {
            rotateModule.SetInteractObject(this);
            _outLine.enabled = true;
        }
        else
        {
            rotateModule.UnSetObject();
            _outLine.enabled = false;
        }
    }
}
