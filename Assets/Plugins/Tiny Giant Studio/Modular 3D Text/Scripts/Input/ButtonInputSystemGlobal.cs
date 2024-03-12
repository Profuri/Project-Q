using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif


namespace TinyGiantStudio.Text
{
    [AddComponentMenu("Tiny Giant Studio/Modular 3D Text/Input System/Global Button Input System", order: 20050)]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/input/button-key/button-input-system")]
    public class ButtonInputSystemGlobal : MonoBehaviour
    {
        #region Variable declaration
        public static ButtonInputSystemGlobal Instance;

        [SerializeField] private ButtonInputProcessorStyle _buttonInputProcessorStyle = ButtonInputProcessorStyle.IndividualPlayerInputComponents;
        public ButtonInputProcessorStyle MyButtonInputProcessorStyle
        {
            get { return _buttonInputProcessorStyle; }
            set
            {
                if (_buttonInputProcessorStyle != value)
                {
#if UNITY_EDITOR
                    if (debugLogs)
                        Debug.Log(value + " being set");
#endif
                    _buttonInputProcessorStyle = value;

                    UpdateButtonInputProcessorScript();
                }

            }
        }
#if ENABLE_INPUT_SYSTEM
        public InputActionAsset inputActionAsset;
#endif
        /// <summary>
        /// This is assigned when MText_InputSystemController is enabled/disabled and when a list with autofocus is instantiated on void start()
        /// </summary>
        public ButtonInputSystemLocal selectedInputSystem;

        public bool debugLogs = false;
        #endregion Variable declaration



        void Awake()
        {
            if (Instance)
                Debug.LogWarning("Multiple MText_UI_ButtonInputProcessor script found on scene.");

            Instance = this;

            UpdateButtonInputProcessorScript();

#if ENABLE_INPUT_SYSTEM
            if (MyButtonInputProcessorStyle != ButtonInputProcessorStyle.CommonInputController)
                return;

            if (inputActionAsset == null)
            {
                ImportantLog("No input action asset selected on ");
                return;
            }
            ButtonInputProcessor inputProcessor = gameObject.GetComponent<ButtonInputProcessor>();

            var map = inputActionAsset.FindActionMap("UI");
            map.Enable();
            map.FindAction("Navigate").performed += inputProcessor.OnNavigate;
            map.FindAction("Submit").performed += inputProcessor.OnSubmit;
#endif
        }


        public void Select(ButtonInputSystemLocal buttonInputSystem)
        {
            selectedInputSystem = buttonInputSystem;
        }
        public void DeSelect(ButtonInputSystemLocal buttonInputSystem)
        {
            if (selectedInputSystem == buttonInputSystem)
                selectedInputSystem = null;
        }


        /// <summary>
        /// Adds/removes and adds all required listeners to button input processor script. 
        /// 
        /// Called in 3 cases.
        /// In Awake()
        /// MyButtonInputProcessorStyle modification
        /// By editor script that doesn't do setup instantly and calls it later because of add component delay
        /// </summary>
        /// <param name="alsoDoSetup"></param>
        public void UpdateButtonInputProcessorScript(bool alsoDoSetup = true)
        {
            if ((MyButtonInputProcessorStyle == ButtonInputProcessorStyle.CommonInputController))
            {
                if (!gameObject.GetComponent<ButtonInputProcessor>())
                    gameObject.AddComponent<ButtonInputProcessor>();

                if (alsoDoSetup)
                    SetupInputProcessor();
            }
            else
            {
                if (gameObject.GetComponent<ButtonInputProcessor>())
                {
                    if (Application.isPlaying)
                        Destroy(gameObject.GetComponent<ButtonInputProcessor>());
                    else
                        DestroyImmediate(gameObject.GetComponent<ButtonInputProcessor>());
                }
            }
        }

        /// <summary>
        /// Attaches required listeners to button input processor
        /// </summary>
        public void SetupInputProcessor()
        {
            ButtonInputProcessor buttonInputProcessor = gameObject.GetComponent<ButtonInputProcessor>();

            if (buttonInputProcessor == null)
                return;

            buttonInputProcessor.upAxisEvent ??= new UnityEvent();
            buttonInputProcessor.downAxisEvent ??= new UnityEvent();
            buttonInputProcessor.leftAxisEvent ??= new UnityEvent();
            buttonInputProcessor.rightAxisEvent ??= new UnityEvent();
            buttonInputProcessor.submitEvent ??= new UnityEvent();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!CheckIfContains(buttonInputProcessor.upAxisEvent, this, "UpAxisEvent"))
                    UnityEventTools.AddPersistentListener(buttonInputProcessor.upAxisEvent, UpAxisEvent);

                if (!CheckIfContains(buttonInputProcessor.downAxisEvent, this, "DownAxisEvent"))
                    UnityEventTools.AddPersistentListener(buttonInputProcessor.downAxisEvent, DownAxisEvent);

                if (!CheckIfContains(buttonInputProcessor.leftAxisEvent, this, "LeftAxisEvent"))
                    UnityEventTools.AddPersistentListener(buttonInputProcessor.leftAxisEvent, LeftAxisEvent);

                if (!CheckIfContains(buttonInputProcessor.rightAxisEvent, this, "RightAxisEvent"))
                    UnityEventTools.AddPersistentListener(buttonInputProcessor.rightAxisEvent, RightAxisEvent);

                if (!CheckIfContains(buttonInputProcessor.submitEvent, this, "SubmitEvent"))
                    UnityEventTools.AddPersistentListener(buttonInputProcessor.submitEvent, SubmitEvent);

                EditorUtility.SetDirty(buttonInputProcessor);
            }
            else
            {
                if (!CheckIfContains(buttonInputProcessor.upAxisEvent, this, "UpAxisEvent"))
                    buttonInputProcessor.upAxisEvent.AddListener(UpAxisEvent);

                if (!CheckIfContains(buttonInputProcessor.downAxisEvent, this, "DownAxisEvent"))
                    buttonInputProcessor.downAxisEvent.AddListener(DownAxisEvent);

                if (!CheckIfContains(buttonInputProcessor.leftAxisEvent, this, "LeftAxisEvent"))
                    buttonInputProcessor.leftAxisEvent.AddListener(LeftAxisEvent);

                if (!CheckIfContains(buttonInputProcessor.rightAxisEvent, this, "RightAxisEvent"))
                    buttonInputProcessor.rightAxisEvent.AddListener(RightAxisEvent);

                if (!CheckIfContains(buttonInputProcessor.submitEvent, this, "SubmitEvent"))
                    buttonInputProcessor.submitEvent.AddListener(SubmitEvent);
            }
#else

            if (!CheckIfContains(buttonInputProcessor.upAxisEvent, this, "UpAxisEvent"))
                buttonInputProcessor.upAxisEvent.AddListener(UpAxisEvent);

            if (!CheckIfContains(buttonInputProcessor.downAxisEvent, this, "DownAxisEvent"))
                buttonInputProcessor.downAxisEvent.AddListener(DownAxisEvent);

            if (!CheckIfContains(buttonInputProcessor.leftAxisEvent, this, "LeftAxisEvent"))
                buttonInputProcessor.leftAxisEvent.AddListener(LeftAxisEvent);

            if (!CheckIfContains(buttonInputProcessor.rightAxisEvent, this, "RightAxisEvent"))
                buttonInputProcessor.rightAxisEvent.AddListener(RightAxisEvent);

            if (!CheckIfContains(buttonInputProcessor.submitEvent, this, "SubmitEvent"))
                buttonInputProcessor.submitEvent.AddListener(SubmitEvent);
#endif
        }

        bool CheckIfContains(UnityEvent myEvent, object target, string targetMethodName)
        {
            myEvent ??= new UnityEvent(); //double check because I am paranoid

            for (int i = 0; i < myEvent.GetPersistentEventCount(); i++)
            {
                if (myEvent.GetPersistentTarget(i) == (object)target)
                {
                    if (myEvent.GetPersistentMethodName(i) == targetMethodName)
                    {
                        if (debugLogs)
                            Debug.Log("Already contains " + targetMethodName);

                        return true;
                    }
                }
            }

            return false;
        }

        public void UpAxisEvent()
        {
            if (IsThereAppropriateTarget())
                selectedInputSystem.upAxisEvent.Invoke();
        }
        public void DownAxisEvent()
        {
            if (IsThereAppropriateTarget())
                selectedInputSystem.downAxisEvent.Invoke();
        }
        public void LeftAxisEvent()
        {
            if (IsThereAppropriateTarget())
                selectedInputSystem.leftAxisEvent.Invoke();
        }
        public void RightAxisEvent()
        {
            if (IsThereAppropriateTarget())
                selectedInputSystem.rightAxisEvent.Invoke();
        }

        public void SubmitEvent()
        {
            if (IsThereAppropriateTarget())
                selectedInputSystem.submitEvent.Invoke();
        }


        bool IsThereAppropriateTarget()
        {
            if (selectedInputSystem == null)
                return false;

            if (!selectedInputSystem.gameObject.activeInHierarchy)
                return false; //if the list is disabled in hierarchy, not an appropriate target

            return true;
        }

        void ImportantLog(string msg)
        {
#if UNITY_EDITOR
            Debug.Log(msg, gameObject);
#else
            if(debugLogs)
                Debug.Log(msg, gameObject);
#endif
        }

        public enum ButtonInputProcessorStyle
        {
            IndividualPlayerInputComponents,
            CommonInputController,
            Custom
        }
    }
}