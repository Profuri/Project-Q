using InteractableSystem;
using UnityEngine;

public abstract class PressureAffectedObject : MonoBehaviour, IInteractable
{
    public Transform GetTransform => transform;
    public EInteractType InteractType => EInteractType.AFFECTED_OTHER;
    public abstract void OnInteraction(PlayerController player, bool interactValue);
}