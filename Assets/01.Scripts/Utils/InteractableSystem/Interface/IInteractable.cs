using UnityEngine;

namespace InteractableSystem
{
    public interface IInteractable
    {
        public void OnInteraction(PlayerController player, bool interactValue);
    }
}