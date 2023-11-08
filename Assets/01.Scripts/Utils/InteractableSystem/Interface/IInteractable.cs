using UnityEngine;

namespace InteractableSystem
{
    public interface IInteractable
    {
        public Transform GetTransform { get; }
        public EInteractType InteractType { get; }
        public void OnInteraction(PlayerController player, bool interactValue);
    }
}