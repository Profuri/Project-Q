using StageStructureConvertSystem;
using UnityEngine;

namespace InteractableSystem
{
    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        public bool InterEnd { get; set; }

        [SerializeField] private EInteractType _interactType;
        public EInteractType InteractType => _interactType;

        [SerializeField] private EInteractableAttribute _attribute;
        public EInteractableAttribute Attribute => _attribute;

        public abstract void OnInteraction(StructureObjectUnitBase communicator, bool interactValue);
    }
}