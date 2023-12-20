using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class LaserToggleObject : InteractableObject
{
    [SerializeField] private InteractableObject _affectedObject;
    
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        if (_affectedObject is not null)
        {
            _affectedObject.OnInteraction(communicator, interactValue);
        }
    }
}
