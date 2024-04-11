using System;
using AxisConvertSystem;
using UnityEngine;

namespace InteractableSystem
{
    public abstract class InteractableObject : ObjectUnit, IInteractable
    {
        public bool InterEnd { get; set; }
        public bool IsInteract { get; set; }
        public bool IsDetected { get; private set; }

        [field: SerializeField] public Vector3 Offset { get; private set; } = Vector3.zero; 

        [SerializeField] private EInteractType _interactType;
        public EInteractType InteractType => _interactType;

        [SerializeField] private EInteractableAttribute _attribute;
        public EInteractableAttribute Attribute => _attribute;

        public virtual void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param) => _communicator = communicator;
        private InteractionMark _interactionMark;

        protected ObjectUnit _communicator;

        protected virtual void OnDisable()
        {
            if (_communicator != null && _communicator is PlayerUnit playerUnit)
            {
                playerUnit.HoldingHandler.Detach();
                OnDetectedLeave();
                _communicator = null;
            }
        }

        public virtual void OnDetectedEnter(ObjectUnit communicator = null)
        {
            IsDetected = true;

            if (_interactionMark == null)
            {
                _interactionMark = SceneControlManager.Instance.AddObject("InteractionMark") as InteractionMark;
                _interactionMark.Setting(this);
            }
        }

        
        public virtual void OnDetectedLeave(ObjectUnit communicator = null)
        {
            IsDetected = false;

            if (_interactionMark != null)
            {
                SceneControlManager.Instance.DeleteObject(_interactionMark);
                _interactionMark = null;
            }
        }


        #if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            //Vector3 center = transform.position;

            //var col = GetComponent<Collider>();
            
            //if (Offset < 0.1f)
            //{
            //    center += new Vector3(0,col.bounds.size.y * 0.7f,0);
            //}
            //else
            //{
            //    center += new Vector3(0, Offset, 0);
            //}

            //Gizmos.color = Color.red;
            //Gizmos.DrawSphere(center,0.2f);
        }
        #endif
    }
}