using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace TinyGiantStudio.Text
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tiny Giant Studio/Modular 3D Text/Horizontal Selector", order: 20006)]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/ui/horizontal-selector")]
    public class HorizontalSelector : MonoBehaviour
    {
        #region Variable declaration

        /// <summary>
        /// The 3D text used to show the current value
        /// </summary>
        [Tooltip("The 3D text used to show the current value")]
        public Modular3DText text;

        /// <summary>
        /// If keyboard control is enabled, selected = you can control via selected
        /// <para>This value will be controlled by list, if it is in one</para>
        /// <para>If you are looking for the selected option, that's the int 'Value'.</para>
        /// </summary>
        [Tooltip("If keyboard control is enabled, selected = you can control via selected. \nThis value will be controlled by list, if it is in one")]
        public bool focused = false;

        /// <summary>
        /// Can this be interacted with. If disabled, can't be selected in list
        /// </summary>
        [Tooltip("If keyboard control is enabled, selected = you can control via selected\nOr selected/deselected in a List")]
        public bool interactable = true;

        /// <summary>
        /// Available options for horizontal selector
        /// </summary>
        [Tooltip("Available options for horizontal selector. \nVariable name: options")]
        public List<string> options = new List<string>
        (
            new string[] { "Option 1", "Option 2", "Option 3" }
        );

        public AudioClip valueChangeSoundEffect;
        public AudioSource audioSource;

        public UnityEvent onSelectEvent;

        [Tooltip("The new value is passed as dynamic value")]
        public UnityEvent<int> onValueChangedEvent;

        public UnityEvent onValueIncreasedEvent;
        public UnityEvent onValueDecreasedEvent;

        [Tooltip("This is to avoid key presses changing values too fast.")]
        public bool limitFastValueChanges = true;

        public float valueChangeMinDelay = 0.5f;
        private float nextValidValueChangeTime = 0;

        #endregion Variable declaration

        #region Logic

        #region Public logics

        //Value variable is also in this region because although it is a variable declaration,
        //this contains all the logic

        [SerializeField]
        [FormerlySerializedAs("value")]
        private int _value;

        /// <summary>
        /// Currently selected option
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                //If no changes, return
                if (_value == value) return;

                //This optional logic stops too fast value changes
                if (!ValidTime()) return;

                //Cache the old value for later
                int oldValue = _value;

                //Apply a valid value change
                _value = ValidValue(value);

                UpdateText();

                onValueChangedEvent.Invoke(_value);

                //Compare the cache and call the events after the new value is assigned,
                //Otherwise, any event being called might be using the old value
                if (value > oldValue)
                    onValueIncreasedEvent.Invoke();
                else
                    if (value < oldValue)
                    onValueDecreasedEvent.Invoke();

                if (audioSource && valueChangeSoundEffect)
                    audioSource.PlayOneShot(valueChangeSoundEffect);
            }
        }

        /// <summary>
        /// Increases the selected number.
        /// <para>If the number is greater/equal(>=) than the options count, sets it to 0</para>
        /// </summary>
        public void Increase()
        {
            Value++;
        }

        /// <summary>
        /// Decreases the selected number.
        /// <para>If the number is less than zero, sets it to max</para>
        /// </summary>
        public void Decrease()
        {
            Value--;
        }

        private int ValidValue(int newValue)
        {
            if (newValue < 0) return options.Count - 1; //If the value is too low, it wraps around to the maximum
            if (newValue >= options.Count) return 0; //If the value is too high, it resets to minimum

            return newValue;
        }

        /// <summary>
        /// Updates current text to match the currently selected value
        /// </summary>
        public void UpdateText()
        {
            if (options.Count == 0 || _value < 0 || options.Count <= _value)
                return;

            if (text)
                text.UpdateText(options[_value]);
            else
            {
                Debug.LogError("No text is attached to Horizontal selector: " + gameObject.name, gameObject);
            }
        }

        /// <summary>
        /// Selects/Deselects the component
        /// </summary>
        /// <param name="enable">true = selected, false = deselected</param>
        public void Focus(bool enable)
        {
            focused = enable;
            if (focused && interactable)
            {
                onSelectEvent.Invoke();
            }
            else this.enabled = false;
        }

        #endregion Public logics

        #region Private Logics

        /// <summary>
        /// This handles the logic behind the limitFastValueChanges variable
        /// This is to avoid key presses changing values too fast. Otherwise,
        /// if the limit is implemented by input system instead of this,
        /// Slider is accidentally slowed down as well.
        /// </summary>
        /// <returns></returns>
        private bool ValidTime()
        {
            if (!limitFastValueChanges) return true;

            if (nextValidValueChangeTime > Time.time) return false;

            nextValidValueChangeTime = Time.time + valueChangeMinDelay;
            return true;
        }

        #endregion Private Logics

        #endregion Logic
    }
}