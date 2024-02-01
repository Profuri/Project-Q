using AxisConvertSystem;
using UnityEngine;

namespace InteractableSystem
{
    public interface IInteractable
    {
        public void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param);
    }
}