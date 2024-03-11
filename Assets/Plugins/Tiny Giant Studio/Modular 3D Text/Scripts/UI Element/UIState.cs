using System;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    /// <summary>
    /// If current state is focused, it won't set state to hovered and unhovered
    /// </summary>
    [System.Serializable]
    public class UIState
    {
        [SerializeField]
        public StateEnum _state;
        public StateEnum State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                    return;

                if (_state == StateEnum.focused && (value == StateEnum.hovered || value == StateEnum.unhovered))
                    return;

                _state = value;

                CallAppropriateMethods();
            }
        }

        public event EventHandler HoverEnter;
        public event EventHandler HoverExit;
        public event EventHandler PressStart;
        public event EventHandler PressComplete;
        public event EventHandler Focused;
        public event EventHandler Unfocused;



        public void CallAppropriateMethods()
        {
            switch (State)
            {
                case StateEnum.hovered:
                    HoverEnter?.Invoke(this, EventArgs.Empty);
                    break;
                case StateEnum.unhovered:
                    HoverExit?.Invoke(this, EventArgs.Empty);
                    State = StateEnum.normal;
                    break;
                case StateEnum.pressStart:
                    PressStart?.Invoke(this, EventArgs.Empty);
                    break;
                case StateEnum.pressComplete:
                    PressComplete?.Invoke(this, EventArgs.Empty);
                    break;
                case StateEnum.focused:
                    Focused?.Invoke(this, EventArgs.Empty);
                    break;
                case StateEnum.unfocused:
                    Unfocused?.Invoke(this, EventArgs.Empty);
                    State = StateEnum.normal;
                    break;
                default:
                    break;
            }
        }











        /// <summary>
        /// Not yet implemented everywhere
        /// https://ferdowsur.gitbook.io/modular-3d-text/utility/ui-states
        /// </summary>
        public enum StateEnum
        {
            /// <summary>
            /// Primary states.
            /// UI will enter these states and stay
            /// </summary>
            normal,
            hovered, //selected
            focused, //if an UI item can take input like type, it enters focus
            disabled,


            /// <summary>
            /// Transition states
            /// UI will move away from these states after entering
            /// </summary>
            press, //Will automatically move to other state after a time period. Example use-case would be key input showing press visual
            pressStart, //this is select start term in XR toolkit
            beingPressed, //The user is holding down this UI. 
            pressComplete, //The user is no longer holding down this UI. //this is select end term in XR toolkit
            unhovered, //no longer hovered. Will move back to appropriate state
            unfocused, //no longer focused. Will move back to appropriate state
        }
    }
}
