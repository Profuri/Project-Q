using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/InputReader/Camera")]
    public class CameraInputReader : InputReader, InputControls.ICameraActions
    {
        public event InputEventListener<Vector2> ChangeOffsetEvent = null;
        
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
            var offset = context.ReadValue<Vector2>();
            ChangeOffsetEvent?.Invoke(offset);
        }
        
        public override void ClearInputEvent()
        {
            ChangeOffsetEvent = null;
        }
    }
}