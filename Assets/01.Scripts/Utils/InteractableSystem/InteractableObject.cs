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

        [field: SerializeField] public float Offset { get; private set; } = 0f; 

        [SerializeField] private EInteractType _interactType;
        public EInteractType InteractType => _interactType;

        [SerializeField] private EInteractableAttribute _attribute;
        public EInteractableAttribute Attribute => _attribute;

        public abstract void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param);

        private InteractionMark _interactionMark;

        public virtual void OnDetectedEnter()
        {
            IsDetected = true;

            if (_interactionMark == null)
            {
                _interactionMark = SceneControlManager.Instance.AddObject("InteractionMark") as InteractionMark;
                _interactionMark.Setting(this);
            }
        }
        
        public virtual void OnDetectedLeave()
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
            Debug.Log("LeagueOfLegends");
            Vector3 center = transform.position;
            if (Offset < 0.1f)
            {
                center += new Vector3(0,Collider.bounds.size.y * 0.7f,0);
            }
            else
            {
                center += new Vector3(0, Offset, 0);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(center,0.2f);
        }
        #endif
    }
}