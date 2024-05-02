using UnityEngine;
using UnityEngine.InputSystem;

namespace InputControl
{
    [CreateAssetMenu(menuName = "SO/InputReader/Timeline")]
    public class TimelineInputReader : InputReader, InputControls.ITimelineActions
    {
        public event InputEventListener OnSpeedUpEvent = null;
        public event InputEventListener CancelSpeedUpEvent = null;
        
        public InputControls.TimelineActions Actions { get; private set; }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            Actions = InputControls.Timeline;
            Actions.SetCallbacks(this);
            
            Actions.Enable();
        }
        
        public override void ClearInputEvent()
        {
            OnSpeedUpEvent = null;
            CancelSpeedUpEvent = null;
        }

        public void OnSpeedUp(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnSpeedUpEvent?.Invoke();
            } 
            
            if (context.canceled)
            {
                CancelSpeedUpEvent?.Invoke();
            }
        }
    }
}