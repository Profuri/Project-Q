using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; //if you are getting error here, please make sure the input system package is installed.
#else

using System.Collections.Generic;

#endif

namespace TinyGiantStudio.Text
{
    [DisallowMultipleComponent]
    public class ButtonInputProcessor : MonoBehaviour
    {
        /// <summary>
        /// How long you have to press a key down for it to register as a second key press
        /// </summary>
        [Tooltip("How long you have to press a key down for it to register as a second key press")]
        public float tickRate = 0.5f;

        public float tickRateSideWays = 0.5f;

        //TODO: convert to c# events
        public UnityEvent upAxisEvent;

        public UnityEvent downAxisEvent;
        public UnityEvent leftAxisEvent;
        public UnityEvent rightAxisEvent;
        public UnityEvent submitEvent;

        private float lastPressedUp;
        private float lastPressedDown;
        private float lastPressedLeft;
        private float lastPressedRight;
        private float lastPressedSubmit;

        private Vector2 axisInput;
#if ENABLE_INPUT_SYSTEM
#else
        private float axisSensitivity = 0.1f;
        private string verticalAxisString = "Vertical";
        private string horizontalAxisString = "Horizontal";
        private string submitString = "Submit";

        [HideInInspector]
        public List<StandardInput> inputs = new List<StandardInput>();

        [System.Serializable]
        public class StandardInput
        {
            public KeyCode key;
            public UnityEvent unityEvent;

            [HideInInspector]
            public float lastPressed;
        }

#endif

        private void Update()
        {
            ProcessAxisInput();
        }

#if ENABLE_INPUT_SYSTEM
        void ProcessAxisInput()
        {
            if (axisInput.y > 0)
                AttemptUp();
            else if (axisInput.y < 0)
                AttemptDown();
            else
            {
                lastPressedDown = 0;
                lastPressedUp = 0;
            }

            if (axisInput.x > 0)
                AttemptRight();
            else if (axisInput.x < 0)
                AttemptLeft();
            else
            {
                lastPressedLeft = 0;
                lastPressedRight = 0;
            }
        }
#else

        private void ProcessAxisInput()
        {
            //If you are having error here, please check if you changed the string for Horizontal axis and change the value of horizontalAxisString accordingly
            if (Input.GetAxis(horizontalAxisString) < -axisSensitivity)
                AttemptLeft();
            else
                lastPressedLeft = 0;

            //If you are having error here, please check if you changed the string for Horizontal axis and change the value of horizontalAxisString accordingly
            if (Input.GetAxis(horizontalAxisString) > axisSensitivity)
                AttemptRight();
            else
                lastPressedRight = 0;

            //If you are having error here, please check if you changed the string for Vertical axis and change the value of verticalAxisString accordingly
            if (Input.GetAxis(verticalAxisString) > axisSensitivity)
                AttemptUp();
            else
                lastPressedUp = 0;

            //If you are having error here, please check if you changed the string for Vertical axis and change the value of verticalAxisString accordingly
            if (Input.GetAxis(verticalAxisString) < -axisSensitivity)
                AttemptDown();
            else
                lastPressedDown = 0;

            if (Input.GetButton(submitString))
            {
                if (tickRate > 0)
                {
                    if (lastPressedSubmit + tickRate < Time.time)
                    {
                        lastPressedSubmit = Time.time;
                        submitEvent.Invoke();
                    }
                }
                else
                {
                    submitEvent.Invoke();
                }
            }
            else
                lastPressedSubmit = 0;
        }

#endif

        #region Take new input

#if ENABLE_INPUT_SYSTEM
        public void OnNavigate(InputAction.CallbackContext context)
        {
            axisInput = context.ReadValue<Vector2>();

            if (axisInput.x == 0)
            {
                lastPressedLeft = 0;
                lastPressedRight = 0;
            }
            else if (axisInput.y == 0)
            {
                lastPressedUp = 0;
                lastPressedDown = 0;
            }
        }
        /// <summary>
        /// Used by Player Input script, made by Unity
        /// </summary>
        /// <param name="value"></param>
        void OnNavigate(InputValue value)
        {
            axisInput = value.Get<Vector2>();
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (!gameObject.activeInHierarchy)
                return;

            submitEvent.Invoke();
        }
        /// <summary>
        /// Used by Player Input script, made by Unity
        /// </summary>
        /// <param name="value"></param>
        void OnSubmit(InputValue value)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (!value.isPressed) //what does this do?
                return;

            submitEvent.Invoke();
        }
#endif

        #endregion Take new input

        #region Process Input

        private void AttemptUp()
        {
            if (tickRate > 0)
            {
                if (lastPressedUp + tickRate < Time.time)
                {
                    lastPressedUp = Time.time;

                    if (upAxisEvent != null)
                        upAxisEvent.Invoke();
                    //else
                    //    Debug.Log(upAxisEvent);
                }
            }
            else
            {
                upAxisEvent.Invoke();
            }
        }

        private void AttemptDown()
        {
            if (tickRate > 0)
            {
                if (lastPressedDown + tickRate < Time.time)
                {
                    lastPressedDown = Time.time;
                    downAxisEvent.Invoke();
                }
            }
            else
            {
                downAxisEvent.Invoke();
            }
        }

        private void AttemptLeft()
        {
            if (tickRate > 0)
            {
                if (lastPressedLeft + tickRateSideWays < Time.time)
                {
                    lastPressedLeft = Time.time;
                    leftAxisEvent.Invoke();
                }
            }
            else
            {
                leftAxisEvent.Invoke();
            }
        }

        private void AttemptRight()
        {
            if (tickRate > 0)
            {
                if (lastPressedRight + tickRateSideWays < Time.time)
                {
                    lastPressedRight = Time.time;
                    rightAxisEvent.Invoke();
                }
            }
            else
            {
                rightAxisEvent.Invoke();
            }
        }

        #endregion Process Input
    }
}