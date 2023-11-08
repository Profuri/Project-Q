using InteractableSystem;
using UnityEngine;

public class GoalObject : MonoBehaviour, IInteractable
{
    public Transform GetTransform => transform;
    public EInteractType InteractType => EInteractType.INPUT_RECEIVE;

    public void OnInteraction(PlayerController player, bool interactValue)
    {
        Debug.Log("Goal!!");    
    }
}
