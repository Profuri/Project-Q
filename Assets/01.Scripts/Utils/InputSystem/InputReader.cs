using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/InputReader")]
    public class InputReader : ScriptableObject, InputControls.IPlayerActions
    {
        public delegate void InputEventListener();
        public delegate void InputEventListener<in T>(T value);
        
        public event InputEventListener OnJumpEvent = null;
        public event InputEventListener OnInteractionEvent = null;
        public event InputEventListener<bool> OnAxisControlEvent = null;
        public event InputEventListener OnClickEvent = null;

        [HideInInspector] public Vector3 movementInput;

        private InputControls _inputControls;
        public InputControls InputControls => _inputControls;

        private void OnEnable()
        {
            if (_inputControls == null)
            {
                _inputControls = new InputControls();
                _inputControls.Player.SetCallbacks(this);
            }
            
            _inputControls.Player.Enable();
        }

        public void SetEnableInput(bool enabled)
        {
            if (enabled)
                _inputControls.Player.Enable();
            else
                _inputControls.Player.Disable();
        }

        public void SetEnableInput(EInputCategory inputCategory)
        {

        }

        public void ClearInputEvent()
        {
            OnJumpEvent = null;
            OnInteractionEvent = null;
            OnAxisControlEvent = null;
            OnClickEvent = null;
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            movementInput = new Vector3(value.x, 0, value.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnJumpEvent?.Invoke();
            }
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInteractionEvent?.Invoke();
            }
        }

        public void OnAxisControl(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnAxisControlEvent?.Invoke(true);   
            }
            else if(context.canceled)
            {
                OnAxisControlEvent?.Invoke(false);
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnClickEvent?.Invoke();
            }
        }
    }
}