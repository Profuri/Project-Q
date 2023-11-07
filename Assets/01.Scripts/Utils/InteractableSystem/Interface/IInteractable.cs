using UnityEngine;

namespace InteractableSystem
{
    public interface IInteractable
    {
        public Transform GetTransform { get; }
        public void OnInteraction(PlayerController player);
    }
}