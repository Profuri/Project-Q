using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/InputReader/Player")]
    public class PlayerInputReader : InputReader, InputControls.IPlayerActions
    {
        public event InputEventListener OnJumpEvent = null;
        public event InputEventListener OnInteractionEvent = null;
        public event InputEventListener<bool> OnAxisControlEvent = null;
        public event InputEventListener OnReloadClickEvent = null;
        public event InputEventListener OnClickEvent = null;
        [HideInInspector] public Vector3 movementInput;
        
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
                OnAxisControlEvent?.Invoke(true);   
            }
            else if(context.canceled)
            {
                OnAxisControlEvent?.Invoke(false);
            }
        }
        
        public void OnReload(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnReloadClickEvent?.Invoke();
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnClickEvent?.Invoke();
            }
        }

        public override void ClearInputEvent()
        {
            OnJumpEvent = null;
            OnInteractionEvent = null;
            OnAxisControlEvent = null;
            OnReloadClickEvent = null;
            OnClickEvent = null;
        }
    }
}