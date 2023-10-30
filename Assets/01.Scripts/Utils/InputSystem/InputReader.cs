using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/New Input System/InputReader")]
    public class InputReader : ScriptableObject, InputControls.IPlayerActions
    {
        public event Action<Vector2> OnMovementEvent = null;
        public event Action OnJumpEvent = null;

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

        public void OnMovement(InputAction.CallbackContext context)
        {
            Vector2 value = context.ReadValue<Vector2>();
            OnMovementEvent?.Invoke(value);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            OnJumpEvent?.Invoke();
        }
    }
}