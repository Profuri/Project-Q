using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/InputReader/Player")]
    public class PlayerInputReader : InputReader, InputControls.IPlayerActions
    {
        public event InputEventListener OnJumpEvent = null;
        public event InputEventListener OnInteractionEvent = null;
        public event InputEventListener OnAxisControlEvent = null;
        public event InputEventListener OnReloadClickEvent = null;
        public event InputEventListener OnAxisConvertEvent = null;
        public event InputEventListener OnPlayerRotateEnterEvent = null;
        public event InputEventListener OnPlayerRotateExitEvent = null;
        
        [HideInInspector] public Vector3 movementInput;
        [HideInInspector] public Vector2 mouseDelta;
        
        public InputControls.PlayerActions Actions { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            Actions = InputControls.Player;
            Actions.SetCallbacks(this);
            
            Actions.Enable();
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
                OnAxisControlEvent?.Invoke();   
            }
            else if(context.canceled)
            {
                OnAxisConvertEvent?.Invoke();
            }
        }
        
        public void OnReload(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnReloadClickEvent?.Invoke();
            }
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnPlayerRotateEnterEvent?.Invoke();
            }
            else if (context.canceled)
            {
                OnPlayerRotateExitEvent?.Invoke();
            }
        }

        public void OnMouseDelta(InputAction.CallbackContext context)
        {
            mouseDelta = context.ReadValue<Vector2>();
        }

        public override void ClearInputEvent()
        {
            OnJumpEvent = null;
            OnInteractionEvent = null;
            OnAxisControlEvent = null;
            OnReloadClickEvent = null;
            OnAxisConvertEvent = null;
            OnPlayerRotateEnterEvent = null;
            OnPlayerRotateExitEvent = null;
        }
    }
}