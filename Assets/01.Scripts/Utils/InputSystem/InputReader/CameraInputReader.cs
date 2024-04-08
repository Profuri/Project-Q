using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/InputReader/Camera")]
    public class CameraInputReader : InputReader, InputControls.ICameraActions
    {
        public InputEventListener OnZoomOutEvent = null;
        public InputEventListener OnZoomInEvent = null;
        
        public InputControls.CameraActions Actions { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Actions = InputControls.Camera;
            Actions.SetCallbacks(this);
            
            Actions.Enable();
        }

        public void OnZoomControl(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnZoomOutEvent?.Invoke();
            }
            else if (context.canceled)
            {
                OnZoomInEvent?.Invoke();
            }
        }

        public override void ClearInputEvent()
        {
            OnZoomOutEvent = null;
            OnZoomInEvent = null;
        }
    }
}