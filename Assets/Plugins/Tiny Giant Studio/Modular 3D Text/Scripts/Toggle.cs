using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace TinyGiantStudio.Text
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tiny Giant Studio/Modular 3D Text/Toggle", order: 20004)]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/ui/toggle")]
    public class Toggle : MonoBehaviour
    {
        [SerializeField]
        private bool _isOn = true;
        public bool IsOn
        {
            get { return _isOn; }
            set { _isOn = value; VisualUpdate(); }
        }
        [Tooltip("The game object that is active when the toggle is on and inactive when the toggle is off. \n\nVariable name: onGraphic")]
        public GameObject onGraphic;
        [Tooltip("The game object that is active when the toggle is off and inactive when the toggle is on. \n\nVariable name: offGraphic")]
        public GameObject offGraphic;

        public UnityEvent onEvent;
        public UnityEvent offEvent;

        /// <summary> 
        /// Sets the activate state according to the parameter passed.
        /// </summary>
        public void Set(bool set)
        {
            IsOn = set;
            VisualUpdate();
            CallToggleEvent();
        }
        /// <summary> 
        /// Switches between on and off.
        /// </summary>
        public void ToggleState()
        {
            IsOn = !IsOn;
            VisualUpdate();
            CallToggleEvent();
        }

        /// <summary>
        /// Calls the correct event according to the state of the toggle
        /// </summary>
        public void CallToggleEvent()
        {
            if (IsOn)
                onEvent.Invoke();
            else
                offEvent.Invoke();
        }

        /// <summary>
        /// Updates the visual of the Toggle to match the 'isOn' variable
        /// </summary>
        public void VisualUpdate()
        {
            if (IsOn) ActiveVisualUpdate();
            else InactiveVisualUpdate();
        }

        /// <summary> 
        /// Changes the graphic to activated.
        /// <para>This only changes the visual. Doesn't update the "active" bool</para>
        /// </summary>
        public void ActiveVisualUpdate()
        {
            SetGraphic(offGraphic, false);
            SetGraphic(onGraphic, true);
        }


        /// <summary> 
        /// Changes the graphic to activated.
        /// <para>This only changes the visual. Doesn't update the "active" bool</para>
        /// </summary>
        public void InactiveVisualUpdate()
        {
            SetGraphic(offGraphic, true);
            SetGraphic(onGraphic, false);
        }

        void SetGraphic(GameObject graphic, bool enable)
        {
            if (graphic)
            {
                graphic.SetActive(enable);
#if UNITY_EDITOR
                Undo.RecordObject(graphic, "Update toggle");
#endif
            }
        }


        /// <summary> 
        /// Editor only. Adds the toggle event to attached button.
        /// <para>Used by menu item when creating a toggle gameobject</para>
        /// </summary>
#if UNITY_EDITOR
        public void AddEventToButton()
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(GetComponent<Button>().pressCompleteEvent, delegate { ToggleState(); });
        }
#endif
    }
}