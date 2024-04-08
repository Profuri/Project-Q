using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/InputReader/UI")]
    public class UIInputReader : InputReader, InputControls.IUIActions
    {
        public event InputEventListener<Vector2> OnLeftClickEvent = null;
        public event InputEventListener<Vector2> OnLeftClickUpEvent = null;
        public event InputEventListener<Vector2> OnMouseMoveEvent = null;
        public event InputEventListener OnUpArrowClickEvent = null;
        public event InputEventListener OnDownArrowClickEvent = null;
        public event InputEventListener OnEnterClickEvent = null;
        public event InputEventListener OnPauseClickEvent = null;
        [HideInInspector] public Vector2 mouseScreenPoint;
        
        public InputControls.UIActions Actions { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            Actions = InputControls.UI;
            Actions.SetCallbacks(this);
            
            Actions.Enable();
        }

        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnLeftClickEvent?.Invoke(mouseScreenPoint);
            }
            else if (context.canceled)
            {
                OnLeftClickUpEvent?.Invoke(mouseScreenPoint);
            }
        }

        public void OnMouseMove(InputAction.CallbackContext context)
        {
            mouseScreenPoint = context.ReadValue<Vector2>();
            OnMouseMoveEvent?.Invoke(mouseScreenPoint);
        }

        public void OnUpArrow(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnUpArrowClickEvent?.Invoke();
            }
        }

        public void OnDownArrow(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnDownArrowClickEvent?.Invoke();
            }
        }

        public void OnEnter(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnEnterClickEvent?.Invoke();
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPauseClickEvent?.Invoke();
            }
        }

        public override void ClearInputEvent()
        {
            OnLeftClickEvent = null;
            OnLeftClickUpEvent = null;
            OnMouseMoveEvent = null;
            OnUpArrowClickEvent = null;
            OnDownArrowClickEvent = null;
            OnEnterClickEvent = null;
            OnPauseClickEvent = null;
        }
    }
}