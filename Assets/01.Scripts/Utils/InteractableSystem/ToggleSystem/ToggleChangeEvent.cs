using UnityEngine.Events;

namespace InteractableSystem
{
    [System.Serializable]
    public class ToggleChangeEvent
    {
        public UnityEvent<bool> invokeEvent;
        public bool inverseValue;

        public void Invoke(bool value)
        {
            if (inverseValue)
            {
                value = !value;
            }
            invokeEvent?.Invoke(value);
        }
    }
}
