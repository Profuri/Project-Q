using AxisConvertSystem;
using InteractableSystem;

[System.Serializable]
public class AffectedObject
{
    public InteractableObject InteractableObject;
    public bool inverseValue;

    public void Invoke(ObjectUnit communicator, bool interactValue)
    {
        if (inverseValue)
        {
            interactValue = !interactValue;
        }
        
        InteractableObject?.OnInteraction(communicator, interactValue);
    }
}