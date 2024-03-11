using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using TinyGiantStudio.Layout;

namespace TinyGiantStudio.Text
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tiny Giant Studio/Modular 3D Text/Slider", order: 20005)]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/ui/slider")]
    public class Slider : MonoBehaviour
    {
        /// <summary>
        /// Selects on Awake()
        /// <para>Selected items can be controlled by keyboard</para>
        /// <para>If it's in a list, this is controlled by list</para>
        /// </summary>
        public bool autoFocusOnGameStart = false;

        /// <summary>
        /// As the name suggests, if it isn't interactable nothing can interact with it and it gets ignored in a list
        /// </summary>
        public bool interactable = true;

        [Tooltip("Minimum value")]
        public float minValue = 0;
        [Tooltip("Maximum value")]
        public float maxValue = 100;
        [SerializeField]
        private float _currentValue = 50; //todo rename
        public float CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                UpdateValue();
            }
        }
        public float CurrentPercentage() => (CurrentValue / maxValue) * 100;


        public SliderHandle handle = null;
        public Renderer handleGraphic = null;
        public Transform progressBar = null;
        public GameObject progressBarPrefab = null;
        public Transform background = null;
        public float backgroundSize = 10;

        /// <summary>
        /// 0 is left to right
        /// </summary>
        public int directionChoice;

        [Tooltip("How much to change on key press")]
        public float keyStep = 150;

        public bool useEvents = true;
        public UnityEvent onValueChanged = null;
        [Tooltip("Mouse/touch dragging the slider ended")]
        public UnityEvent sliderDragEnded = null;

        //visual
        public Material selectedHandleMat = null;
        public Material unSelectedHandleMat = null;
        public Material clickedHandleMat = null;
        public Material disabledHandleMat = null;

        public bool useValueRangeEvents = true;
        public bool usePercentage = true;
        /// <summary>
        /// Events are called when the slider value enters a specific range
        /// Checks value in the top to down order. If you have two ranges that can be fulfilled simultaneously, the first one gets called.
        /// </summary>
        public List<ValueRange> valueRangeEvents = new List<ValueRange>();
        [HideInInspector][SerializeField] int lastValue = 0;

        [System.Serializable]
        public class ValueRange
        {
            public float min = 0;
            public float max = 25;
            /// <summary>
            /// Enabled and disabled when within range
            /// </summary>
            public GameObject icon;
            public bool triggeredAlready;
            public UnityEvent oneTimeEvents;
            public UnityEvent repeatEvents;
        }



        #region Unity Things
        void Awake()
        {
            if (interactable && autoFocusOnGameStart && !StaticMethods.GetParentList(transform))
            {
                Focus(true);
            }
            else
            {
                DisabledVisual();
                this.enabled = false;
            }
        }
        #endregion Unity Things


        /// <summary>
        /// This can be used to udate the slider's graphic and call the appropriate events incase they weren't called.
        /// </summary>
        public void UpdateValue()
        {
            ValueChanged();
            UpdateGraphic();
        }

        /// <summary>
        /// Updates the value of the slider
        /// This is same as setting the CurrentValue directly
        /// </summary>
        /// <param name="newValue">The parameter is the new value of the slider</param>
        public void UpdateValue(int newValue)
        {
            CurrentValue = newValue;
        }

        /// <summary>
        /// Updates the value of the slider.
        /// This is same as setting the CurrentValue directly
        /// </summary>
        /// <param name="newValue">The parameter is the new value of the slider</param>
        public void UpdateValue(float newValue)
        {
            CurrentValue = newValue;
        }

        /// <summary>
        /// Increases the value of the slider
        /// </summary>
        public void IncreaseValue()
        {
            float newValue = CurrentValue + (Time.deltaTime * keyStep);
            if (newValue > maxValue)
                newValue = maxValue;

            CurrentValue = newValue;
        }

        /// <summary>
        /// Increases the value of the slider by the given amount
        /// </summary>
        public void IncreaseValue(int amount)
        {
            float newValue = CurrentValue + amount * Time.deltaTime;

            if (newValue > maxValue)
                newValue = maxValue;

            CurrentValue = newValue;
        }
        /// <summary>
        /// Increases the value of the slider by the given amount
        /// </summary>
        public void IncreaseValue(float amount)
        {
            float newValue = CurrentValue + amount * Time.deltaTime;

            if (newValue > maxValue)
                newValue = maxValue;

            CurrentValue = newValue;
        }


        public void DecreaseValue()
        {
            float newValue = CurrentValue - (Time.deltaTime * keyStep);
            if (newValue < minValue)
                newValue = minValue;

            CurrentValue = newValue;
        }
        public void DecreaseValue(int amount)
        {
            float newValue = CurrentValue - amount * Time.deltaTime;
            if (newValue < minValue)
                newValue = minValue;

            CurrentValue = newValue;
        }
        public void DecreaseValue(float amount)
        {
            float newValue = CurrentValue - amount * Time.deltaTime;
            if (newValue < minValue)
                newValue = minValue;

            CurrentValue = newValue;
        }
        /// <summary>
        /// Selects/deselects slider
        /// </summary>
        /// <param name="enable"></param>
        public void Focus(bool enable)
        {
            this.enabled = enable;
#if ENABLE_INPUT_SYSTEM
            if (GetComponent<PlayerInput>())
            {
                if (enable)
                    GetComponent<PlayerInput>().ActivateInput();
            }
#endif

            if (enable)
                SelectedVisual();
            else
                UnSelectedVisual();
        }

        public void SelectedVisual()
        {
            var applySelectedStyle = ApplySelectedStyleFromParent();

            if (applySelectedStyle.Item1)
                ApplyVisual(applySelectedStyle.Item2.SelectedBackgroundMaterial);
            else
                ApplyVisual(selectedHandleMat);
        }
        public void UnSelectedVisual()
        {
            var applyUnselectedStyle = ApplyNormalStyleFromParent();

            if (applyUnselectedStyle.Item1)
                ApplyVisual(applyUnselectedStyle.Item2.NormalBackgroundMaterial);
            else
                ApplyVisual(unSelectedHandleMat);
        }
        public void ClickedVisual()
        {
            var applyPressedStyle = ApplyPressedStyleFromParent();

            if (applyPressedStyle.Item1)
                ApplyVisual(applyPressedStyle.Item2.PressedBackgroundMaterial);
            else
                ApplyVisual(clickedHandleMat);
        }
        public void DisabledVisual()
        {
            var applyDisabledStyle = ApplyDisabledStyleFromParent();

            if (applyDisabledStyle.Item1)
                ApplyVisual(applyDisabledStyle.Item2.DisabledBackgroundMaterial);
            else
                ApplyVisual(disabledHandleMat);
        }

        void ApplyVisual(Material handleMaterial)
        {
            if (handleGraphic)
                handleGraphic.material = handleMaterial;
        }

        public List GetParentList() => StaticMethods.GetParentList(transform);

        public (bool, List) ApplyNormalStyleFromParent()
        {
            List list = GetParentList();
            if (list)
            {
                if (list.UseStyle && list.UseNormalItemVisual)
                {
                    return (true, list);
                }
            }
            //don't apply from list
            return (false, list);
        }
        public (bool, List) ApplySelectedStyleFromParent()
        {
            //get style from parent list
            List list = GetParentList();
            if (list)
            {
                if (list.UseStyle && list.UseSelectedItemVisual)
                {
                    return (true, list);
                }
            }
            //don't apply from list
            return (false, list);
        }
        public (bool, List) ApplyPressedStyleFromParent()
        {
            //get style from parent list
            List list = GetParentList();
            if (list)
            {
                if (list.UseStyle && list.UsePressedItemVisual)
                {
                    return (true, list);
                }
            }
            //don't apply from list
            return (false, list);
        }
        public (bool, List) ApplyDisabledStyleFromParent()
        {
            List list = GetParentList();

            if (list)
            {
                if (list.UseStyle && list.UseDisabledItemVisual)
                    return (true, list);
            }
            return (false, list);
        }

        /// <summary>
        /// Calls events after value is changed
        /// </summary>
        public void ValueChanged()
        {
            if (useEvents)
                onValueChanged.Invoke();
            if (useValueRangeEvents)
                ValueRangeEvents();
        }
        void ValueRangeEvents()
        {
            //two lines can be rewritten as one
            float valueToCheckAgainst = CurrentValue;
            if (usePercentage) valueToCheckAgainst = CurrentPercentage();

            bool newRange = false;
            int newValue = 0;
            for (int i = 0; i < valueRangeEvents.Count; i++)
            {
                //correct range
                if (valueToCheckAgainst >= valueRangeEvents[i].min && valueToCheckAgainst <= valueRangeEvents[i].max)
                {
                    newValue = i;
                    if (lastValue != i) newRange = true;

                    break;
                }
            }
            if (newRange && valueRangeEvents.Count > lastValue)
            {
                if (valueRangeEvents[lastValue].icon) valueRangeEvents[lastValue].icon.SetActive(false);
                lastValue = newValue;
            }
            ProcessSelectedValueRange(newValue, newRange);
        }
        void ProcessSelectedValueRange(int i, bool firstTime)
        {
            if (valueRangeEvents.Count <= i)
                return;

            if (valueRangeEvents[i].icon) valueRangeEvents[i].icon.SetActive(true);

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (firstTime) valueRangeEvents[i].oneTimeEvents.Invoke();
                valueRangeEvents[i].repeatEvents.Invoke();
            }
#else
            if (firstTime) valueRangeEvents[i].oneTimeEvents.Invoke();
            valueRangeEvents[i].repeatEvents.Invoke();
#endif
        }


        /// <summary>
        /// Used by raycast selector to call events after dragging handle ended
        /// </summary>
        public void ValueChangeEnded()
        {
            if (useEvents)
                sliderDragEnded.Invoke();
        }


        /// <summary>
        /// Sets slider to uninteractable
        /// </summary>
        public void Uninteractable()
        {
            interactable = false;
            DisabledVisual();
        }
        /// <summary>
        /// Sets slider to interactable
        /// </summary>
        public void Interactable()
        {
            interactable = true;
            UnSelectedVisual();
        }

        /// <summary>
        /// Creates new value range event
        /// </summary>
        public void NewValueRange()
        {
            ValueRange valueRange = new ValueRange();
            valueRangeEvents.Add(valueRange);
        }

        /// <summary>
        /// Updates the value of the slider according to position of the handle
        /// Used by raycast selector to update the value after dragging handle
        /// </summary>
        public void GetCurrentValueFromHandle()
        {
            CurrentValue = RangeConvertedValue(handle.transform.localPosition.x, (-backgroundSize / 2), (backgroundSize / 2), minValue, maxValue);
            UpdateProgressBar();
        }



        /// <summary>
        /// Updates the graphic of slider to match the value
        /// </summary>
        public void UpdateGraphic()
        {
            UpdateHandle();
            UpdateProgressBar();
        }

        void UpdateHandle()
        {
            if (handle)
            {
                int multiplier = -1;
                if (directionChoice == 1) multiplier = 1;

                Vector3 pos = handle.transform.localPosition;
                pos.x = multiplier * RangeConvertedValue(CurrentValue, minValue, maxValue, backgroundSize / 2, -backgroundSize / 2);

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Undo.RecordObject(handle.transform, "Update slider");
#endif

                handle.transform.localPosition = pos;
            }
        }
        void UpdateProgressBar()
        {
            if (!progressBar)
                return;

            Vector3 scale = progressBar.localScale;
            scale.x = ((CurrentValue - minValue) / (maxValue - minValue)) * backgroundSize;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Undo.RecordObject(progressBar, "Update slider");
#endif
            progressBar.localScale = scale;

            Vector3 pos = progressBar.localPosition;
            pos.x = -backgroundSize / 2;
            progressBar.localPosition = pos;
        }

        float RangeConvertedValue(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
        {
            return (((oldValue - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
        }

        public void UpdateBackgroundSize()
        {
            Bounds bounds = MeshBaseSize.CheckMeshSize(background.GetComponent<MeshFilter>().sharedMesh);
            background.localScale = new Vector3((1 / bounds.size.x) * backgroundSize, background.localScale.y, background.localScale.z);
            background.localPosition = new Vector3(bounds.center.x, 0, 0);
            background.localRotation = Quaternion.identity;
        }
    }
}