using InteractableSystem;
using UnityEngine;

public class GoalObject : MonoBehaviour, IInteractable
{
    public Transform GetTransform => transform;
    
    public void OnInteraction(PlayerController player)
    {
        Debug.Log("Goal!!");    
    }
}
