using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/New Input System/InputReader")]
    public class InputReader : ScriptableObject, InputControls.IPlayerActions
    {
        public delegate void InputEventListener();
        public delegate void InputEventListener<in T>(T value);
        
        public event InputEventListener<Vector2> OnMovementEvent = null;
        public event InputEventListener<Vector2> OnRotateEvent = null;
        public event InputEventListener OnJumpEvent = null;
        public event InputEventListener OnAxisControlEvent = null;
        public event InputEventListener OnInteractionEvent = null;

        private InputControls _inputControls;

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

        public void OnMovement(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();
            OnMovementEvent?.Invoke(value);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnJumpEvent?.Invoke();
            }
        }

        public void OnAxisControl(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                OnAxisControlEvent?.Invoke();
            }
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInteractionEvent?.Invoke();
            }
        }

        public void OnRotateObj(InputAction.CallbackContext context)
        {
          
            Vector2 value = context.ReadValue<Vector2>();
            OnRotateEvent?.Invoke(value);
        }
    }
}