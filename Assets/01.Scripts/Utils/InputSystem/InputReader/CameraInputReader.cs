using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/InputReader/Camera")]
    public class CameraInputReader : InputReader, InputControls.ICameraActions
    {
        public event InputEventListener<Vector2> OnChangeOffsetEvent = null;
        
        public InputControls.CameraActions Actions { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Actions = InputControls.Camera;
            Actions.SetCallbacks(this);
            
            Actions.Enable();
        }

        public void OnChangeOffset(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var offset = context.ReadValue<Vector2>();
                OnChangeOffsetEvent?.Invoke(offset);
            }
            else if (context.canceled)
            {
                OnChangeOffsetEvent?.Invoke(Vector3.zero);
            }
        }
        
        public override void ClearInputEvent()
        {
            OnChangeOffsetEvent = null;
        }
    }
}