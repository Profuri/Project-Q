using AxisConvertSystem;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableSystem
{
    public class DefaultInteractableObject : InteractableObject
    {
        [SerializeField] private UnityEvent onInteractEvent;
        
        public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
        {
            onInteractEvent?.Invoke();
        }
    }
}