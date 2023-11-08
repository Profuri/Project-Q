using InteractableSystem;
using UnityEngine;

public abstract class PressureAffectedObject : InteractableObject
{
    public abstract override void OnInteraction(PlayerController player, bool interactValue);
}