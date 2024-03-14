using AxisConvertSystem;
using UnityEngine;

namespace InteractableSystem
{
    public abstract class InteractableObject : ObjectUnit, IInteractable
    {
        public bool InterEnd { get; set; }
        public bool IsInteract { get; set; }
        public bool IsDetected { get; private set; }

        [SerializeField] private EInteractType _interactType;
        public EInteractType InteractType => _interactType;

        [SerializeField] private EInteractableAttribute _attribute;
        public EInteractableAttribute Attribute => _attribute;

        public abstract void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param);

        public virtual void OnDetectedEnter()
        {
            IsDetected = true;
        }
        
        public virtual void OnDetectedLeave()
        {
            IsDetected = false;
        }
    }
}