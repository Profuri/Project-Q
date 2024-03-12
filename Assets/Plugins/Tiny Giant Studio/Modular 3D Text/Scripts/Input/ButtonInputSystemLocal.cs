using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if UNITY_EDITOR

using UnityEditor.Events;

#endif

namespace TinyGiantStudio.Text
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tiny Giant Studio/Modular 3D Text/Input System/Local Button Input System", order: 20051)]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/input/button-key/button-input-system")]
    public class ButtonInputSystemLocal : MonoBehaviour
    {
        public UnityEvent upAxisEvent;
        public UnityEvent downAxisEvent;
        public UnityEvent leftAxisEvent;
        public UnityEvent rightAxisEvent;
        public UnityEvent submitEvent;

        private ButtonInputSystemGlobal buttonInputSystemGlobal;
        private bool useCommonControls;

        //note to self: is called after on enable
        private void Start()
        {
            if (buttonInputSystemGlobal == null) //if this GameObject pre-exists in scene, it may call OnEnable before the global instance has been created.
            {
                GetCommonControlSettings();
                SelectThis();
            }

            UpdateButtonInputProcessorScript();
        }

        private void OnEnable()
        {
            GetCommonControlSettings();
            SelectThis();
        }

        private void OnDisable()
        {
            if (buttonInputSystemGlobal == null)
                return;

            buttonInputSystemGlobal.DeSelect(this);
        }

        private void GetCommonControlSettings()
        {
            if (buttonInputSystemGlobal == null)
                buttonInputSystemGlobal = ButtonInputSystemGlobal.Instance;

            if (buttonInputSystemGlobal == null)
                return;

            if (buttonInputSystemGlobal.MyButtonInputProcessorStyle == ButtonInputSystemGlobal.ButtonInputProcessorStyle.CommonInputController)
                useCommonControls = true;
            else
                useCommonControls = false;
        }

        private void SelectThis()
        {
            if (buttonInputSystemGlobal == null)
                return;

            buttonInputSystemGlobal.Select(this);
        }

        private void UpdateButtonInputProcessorScript()
        {
            if (useCommonControls)
            {
#if ENABLE_INPUT_SYSTEM
                if (GetComponent<PlayerInput>())
                {
                    if (Application.isPlaying)
                        Destroy(gameObject.GetComponent<PlayerInput>());
                    else
                        DestroyImmediate(gameObject.GetComponent<PlayerInput>());
                }
#endif
                if (gameObject.GetComponent<ButtonInputProcessor>())
                {
                    if (Application.isPlaying)
                        Destroy(gameObject.GetComponent<ButtonInputProcessor>());
                    else
                        DestroyImmediate(gameObject.GetComponent<ButtonInputProcessor>());
                }
            }
            else
            {
                if (!gameObject.GetComponent<ButtonInputProcessor>())
                {
                    gameObject.AddComponent<ButtonInputProcessor>();
                    SetupInputProcessor();
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

                UnityEditor.EditorUtility.SetDirty(buttonInputProcessor);
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

        private bool CheckIfContains(UnityEvent myEvent, object target, string targetMethodName)
        {
            myEvent ??= new UnityEvent(); //double check because I am paranoid

            for (int i = 0; i < myEvent.GetPersistentEventCount(); i++)
            {
                if (myEvent.GetPersistentTarget(i) == (object)target)
                {
                    if (myEvent.GetPersistentMethodName(i) == targetMethodName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void UpAxisEvent()
        {
            if (IsThereAppropriateTarget())
                upAxisEvent.Invoke();
        }

        public void DownAxisEvent()
        {
            if (IsThereAppropriateTarget())
                downAxisEvent.Invoke();
        }

        public void LeftAxisEvent()
        {
            if (IsThereAppropriateTarget())
                leftAxisEvent.Invoke();
        }

        public void RightAxisEvent()
        {
            if (IsThereAppropriateTarget())
                rightAxisEvent.Invoke();
        }

        public void SubmitEvent()
        {
            if (IsThereAppropriateTarget())
                submitEvent.Invoke();
        }

        private bool IsThereAppropriateTarget()
        {
            return (gameObject.activeInHierarchy);
        }
    }
}