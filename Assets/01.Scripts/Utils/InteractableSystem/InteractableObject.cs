using AxisConvertSystem;
using UnityEngine;

namespace InteractableSystem
{
    public abstract class InteractableObject : ObjectUnit, IInteractable
    {
        public bool IsInteract { get; set; }
        public bool IsDetected { get; private set; }

        [field: SerializeField] public Vector3 Offset { get; private set; } = Vector3.zero; 

        [SerializeField] private EInteractType _interactType;
        public EInteractType InteractType => _interactType;

        [SerializeField] private EInteractableAttribute _attribute;
        public EInteractableAttribute Attribute => _attribute;

        private InteractionMark _interactionMark;

        public override void OnPush()
        {
            base.OnPush();
            OnDetectedLeave();
        }

        public virtual void OnDetectedEnter(ObjectUnit communicator = null)
        {
            IsDetected = true;

            if (_interactType == EInteractType.INPUT_RECEIVE && _interactionMark == null)
            {
                _interactionMark = SceneControlManager.Instance.AddObject("InteractionMark") as InteractionMark;
                _interactionMark.Setting(this);
            }
        }

        public virtual void OnDetectedLeave(ObjectUnit communicator = null)
        {
            IsDetected = false;

            if (_interactType == EInteractType.INPUT_RECEIVE && _interactionMark != null)
            {
                SceneControlManager.Instance.DeleteObject(_interactionMark);
                _interactionMark = null;
            }
        }

        public abstract void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param);
    }
}