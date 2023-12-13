using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class GoalObject : InteractableObject
{
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        Debug.Log("Goal!!");    
    }
}
