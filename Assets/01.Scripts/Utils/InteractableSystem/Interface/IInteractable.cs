using AxisConvertSystem;
using UnityEngine;

namespace InteractableSystem
{
    public interface IInteractable
    {
        public void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param);
    }
}