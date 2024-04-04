using UnityEngine;

namespace InputControl
{
    public abstract class InputReader : ScriptableObject
    {
        public delegate void InputEventListener();
        public delegate void InputEventListener<in T>(T value);

        private InputControls _inputControls;
        public InputControls InputControls => _inputControls;

        protected virtual void OnEnable()
        {
            if (_inputControls == null)
            {
                _inputControls = new InputControls();
            }
        }

        public abstract void ClearInputEvent();
    }
}