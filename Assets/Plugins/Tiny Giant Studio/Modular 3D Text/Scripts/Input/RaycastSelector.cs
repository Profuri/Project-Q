using UnityEngine;


namespace TinyGiantStudio.Text
{
    /// <summary>
    /// This component is used to cast a ray from camera to interact with 3D UI Elements. Handles the logic part only. Not the input part.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Tiny Giant Studio/Modular 3D Text/Input System/Raycast Selector", order: 20052)]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/input/mouse-touch/raycast-selector")]
    public class RaycastSelector : MonoBehaviour
    {
        #region Variable Declaration--------------------------

        #region Raycast settings
        [SerializeField] LayerMask UILayer = ~0;
        [SerializeField] float maxRayDistance = 5000;
        #endregion Raycast settings

        #region Behavior Settings
        [Tooltip("True = How normal UI works. It toggles if clicking a inputfield enables it " +
            "and clicking somewhere else disables it")]
        public bool onlyOneTargetFocusedAtOnce = true;

        [Tooltip("Unhovering mouse from a Btn will unselect it")]
        public bool unselectBtnOnUnhover = true;
        #endregion Behavior Settings


        Transform clickedTarget = null;

        #endregion Variable Declaration--------------------------


        /// <summary>
        /// Recieves ray
        /// </summary>
        /// <param name="ray"></param>
        /// <returns>What was hit</returns>
        public Transform RaycastCheck(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, UILayer))
                return hit.transform;

            return null;
        }

        public void PressTarget(Transform hit)
        {
            if (onlyOneTargetFocusedAtOnce)
                UnFocusPreviouslySelectedItems(hit);

            PressInputString(hit);
            PressButton(hit);
            PressSlider(hit);
        }
        void PressInputString(Transform hit)
        {
            InputField inputString = hit.gameObject.GetComponent<InputField>();
            if (!InteractWithInputString(inputString))
                return;

            inputString.Select();
            clickedTarget = hit;
        }
        void PressSlider(Transform hit)
        {
            SliderHandle sliderHandle = hit.gameObject.GetComponent<SliderHandle>();
            if (!InteractWithSlider(sliderHandle))
                return;

            hit.gameObject.GetComponent<SliderHandle>().slider.ClickedVisual();
        }
        void PressButton(Transform hit)
        {
            Button button = hit.gameObject.GetComponent<Button>();
            if (!InteractWithButton(button))
                return;

            button.PressButtonVisualUpdate();
        }


        void UnFocusPreviouslySelectedItems(Transform hit)
        {
            if (hit != clickedTarget)
            {
                if (clickedTarget)
                {
                    if (clickedTarget.gameObject.GetComponent<InputField>())
                    {
                        if (clickedTarget.gameObject.GetComponent<InputField>().interactable)
                        {
                            clickedTarget.gameObject.GetComponent<InputField>().Focus(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Selects the 3D UI passed as parameter
        /// </summary>
        /// <param name="target"></param>
        public void SelectTarget(Transform target)
        {
            SelectButton(target);
            SelectSlider(target);
        }
        void SelectSlider(Transform hit)
        {
            SliderHandle sliderHandle = hit.gameObject.GetComponent<SliderHandle>();
            if (!InteractWithSlider(sliderHandle))
                return;

            sliderHandle.slider.SelectedVisual();
        }
        void SelectButton(Transform hit)
        {
            Button button = hit.gameObject.GetComponent<Button>();
            if (!InteractWithButton(button))
                return;

            button.SelectButton();
        }



        #region Unselect
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void UnselectTarget(Transform target)
        {
            if (!target)
                return;

            UnselectButton(target);
            UnselectSlider(target);
        }
        void UnselectSlider(Transform hit)
        {
            SliderHandle sliderHandle = hit.gameObject.GetComponent<SliderHandle>();
            if (!InteractWithSlider(sliderHandle))
                return;

            sliderHandle.slider.UnSelectedVisual();
        }
        void UnselectButton(Transform hit)
        {
            Button button = hit.gameObject.GetComponent<Button>();
            if (!InteractWithButton(button))
                return;

            if (unselectBtnOnUnhover)
            {
                List list = StaticMethods.GetParentList(button.transform);
                if (!list)
                    button.UnselectButton();
                else
                    list.UnselectEverythingDontChangeSelectedItemValue();
            }
            else
            {
                //button.UnselectButton();
            }
        }
        #endregion Unselect


        #region Drag

        #region Dragging
        public void Dragging(Transform hit, Vector3 cursorPosition)
        {
            DragSlider(hit, cursorPosition);
            DragButton(hit);
        }

        void DragSlider(Transform hit, Vector3 cursorPosition)
        {
            SliderHandle sliderHandle = hit.gameObject.GetComponent<SliderHandle>();
            if (!InteractWithSlider(sliderHandle))
                return;

            //cursorPosition in slider handle's local space
            Vector3 localPosition = hit.parent.InverseTransformPoint(cursorPosition); //used to be hit.inverseTransformPoint

            //Remove Y Z position from handle
            localPosition = new Vector3(localPosition.x, 0, 0);

            float size = sliderHandle.slider.backgroundSize;
            localPosition.x = Mathf.Clamp(localPosition.x, -size / 2, size / 2);

            hit.localPosition = localPosition;

            sliderHandle.slider.GetCurrentValueFromHandle();
            sliderHandle.slider.ValueChanged();
        }
        void DragButton(Transform hit)
        {
            Button button = hit.gameObject.GetComponent<Button>();
            if (!InteractWithButton(button))
                return;
            button.ButtonBeingPressed();
        }

        #endregion Dragging

        #region Drag End
        public void DragEnded(Transform hit, Transform currentTarget)
        {
            DragEndOnSlider(hit);
            DragEndOnButton(hit, currentTarget);
        }
        void DragEndOnSlider(Transform hit)
        {
            SliderHandle sliderHandle = hit.gameObject.GetComponent<SliderHandle>();
            if (!InteractWithSlider(sliderHandle))
                return;

            sliderHandle.slider.ValueChangeEnded();
        }
        void DragEndOnButton(Transform hit, Transform currentTarget)
        {
            Button button = hit.gameObject.GetComponent<Button>();
            if (!InteractWithButton(button))
                return;

            if (currentTarget != hit) button.isSelected = false;
            button.PressCompleted();
        }
        #endregion Drag End

        #endregion Drag



        #region CanItIteractWithIt
        bool InteractWithButton(Button button)
        {
            if (!button)
                return false;
            if (button.interactable && button.interactableByMouse)
                return true;

            return false;
        }
        bool InteractWithSlider(SliderHandle sliderHandle)
        {
            if (!sliderHandle)
                return false;
            if (sliderHandle.slider && sliderHandle.slider.interactable)
                return true;

            return false;
        }
        bool InteractWithInputString(InputField inputString)
        {
            if (!inputString)
                return false;
            if (inputString.interactable)
                return true;

            return false;
        }
        #endregion CanItIteractWithIt
    }
}