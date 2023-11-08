using InteractableSystem;
using UnityEngine;

public class GoalObject : InteractableObject
{
    public override void OnInteraction(PlayerController player, bool interactValue)
    {
        Debug.Log("Goal!!");    
    }
}
