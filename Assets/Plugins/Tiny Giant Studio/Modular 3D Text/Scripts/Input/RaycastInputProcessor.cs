using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif


namespace TinyGiantStudio.Text
{
    /// <summary>
    /// Handles input for raycast selector
    /// </summary>
    [AddComponentMenu("Tiny Giant Studio/Modular 3D Text/Input System/Raycast Input Processor", order: 20052)]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/input/mouse-touch/raycast-input-processor")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RaycastSelector))]
    public class RaycastInputProcessor : MonoBehaviour
    {
        #region Raycast settings
        [Tooltip("If not assigned, it will automatically get Camera.main on Start")]
        public Camera myCamera;
        #endregion Raycast settings


        public Transform pointerOnUI;
        public Transform currentTarget;

        bool dragging = false;

        RaycastSelector raycastSelector;


        #region Unity Things
        void Awake()
        {
            raycastSelector = GetComponent<RaycastSelector>();
#if ENABLE_INPUT_SYSTEM
            EnhancedTouchSupport.Enable();
#endif
        }

        void Start()
        {
            //If no camera assigned, get Camera.main
            if (!myCamera)
            {
                myCamera = Camera.main;
                if (!myCamera)
                    Debug.Log("No camera selected for 3D UI Raycaster");
            }
        }

        void Update()
        {
            if (!myCamera)
                return;

            //If Already dragging stuff, do dragging stuff
            if (dragging)
            {
                Dragging();
                DetectDragEnd();
            }
            else
            {
                SelectPress();
            }
        }

        /// <summary>
        /// Select or press
        /// </summary>
        void SelectPress()
        {
            //Check if mouse is on something
            pointerOnUI = RaycastCheck();

            //If mouse not on the old UI, unselect old one
            if (pointerOnUI != currentTarget)
                raycastSelector.UnselectTarget(currentTarget);


            //If mouse on a UI
            if (pointerOnUI)
            {
                //If it's a new target, select that
                if (pointerOnUI != currentTarget)
                    raycastSelector.SelectTarget(pointerOnUI);

                //If the UI is clicked 
                if (PressedButton())
                {
                    raycastSelector.PressTarget(pointerOnUI);
                    dragging = true;
                }
            }

            currentTarget = pointerOnUI;
        }
        #endregion Unity things


        void Dragging()
        {
            Vector3 screenPoint = myCamera.WorldToScreenPoint(currentTarget.position);

#if ENABLE_INPUT_SYSTEM
            //Get the mouse position on screen
            Vector3 cursorScreenPoint = new Vector3(Pointer.current.position.ReadValue().x, Pointer.current.position.ReadValue().y, screenPoint.z);
#else
            //Get the mouse position on screen
            Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
#endif
            //Convert cursor position to world position
            Vector3 cursorPosition = myCamera.ScreenToWorldPoint(cursorScreenPoint);

            raycastSelector.Dragging(currentTarget, cursorPosition);
        }

        bool PressedButton()
        {
#if ENABLE_INPUT_SYSTEM
            if (MouseClicked() || Tapped())
                return true;
            return false;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

#if ENABLE_INPUT_SYSTEM
        bool MouseClicked()
        {
            if (Mouse.current != null)
                return Mouse.current.leftButton.wasPressedThisFrame;

            return false;
        }

        bool Tapped()
        {
            if (Touch.activeTouches.Count > 0)
                return Touch.activeTouches[0].ended;

            return false;
        }
#endif

        Transform RaycastCheck()
        {
#if ENABLE_INPUT_SYSTEM
            Ray ray = myCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
#else
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
#endif
            return raycastSelector.RaycastCheck(ray);
        }

        void DetectDragEnd()
        {
            if (MouseButtonReleased() && dragging)
            {
                dragging = false;
                raycastSelector.DragEnded(currentTarget, RaycastCheck());
            }

            if (!Input.touchSupported)
                return;

            if (Input.touchCount > 0)
            {
#if ENABLE_INPUT_SYSTEM
                if (Input.touches[0].phase == UnityEngine.TouchPhase.Ended)
#else
                if (Input.touches[0].phase == TouchPhase.Ended)
#endif
                {
                    dragging = false;
                    raycastSelector.DragEnded(currentTarget, RaycastCheck());
                }
            }
            else
            {
                dragging = false;
                raycastSelector.DragEnded(currentTarget, RaycastCheck());
            }
        }

        bool MouseButtonReleased()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
                return Mouse.current.leftButton.wasReleasedThisFrame;
            return false;
#else
            return Input.GetMouseButtonUp(0);
#endif
        }
    }
}
