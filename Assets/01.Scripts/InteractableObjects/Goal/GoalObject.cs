using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class GoalObject : InteractableObject
{
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue)
    {
        Debug.Log("Goal!!");    
    }
}
