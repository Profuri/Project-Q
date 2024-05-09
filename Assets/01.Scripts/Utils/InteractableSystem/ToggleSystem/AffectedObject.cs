using AxisConvertSystem;
using InteractableSystem;

namespace InteractableSystem
{
    [System.Serializable]
    public class AffectedObject
    {
        public InteractableObject interactableObject;
        public bool inverseValue;

        public void Invoke(ObjectUnit communicator, bool interactValue)
        {
            if (inverseValue)
            {
                interactValue = !interactValue;
            }
            
            interactableObject?.OnInteraction(communicator, interactValue);

            if (interactValue)
            {
                //interactableObject?.ShowSelectedBorder();
            }
            //interactableObject?.SelectedBorderActivate(interactValue);
        }
    }
}
