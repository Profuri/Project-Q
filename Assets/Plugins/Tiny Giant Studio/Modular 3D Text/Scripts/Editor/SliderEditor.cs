using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using TinyGiantStudio.EditorHelpers;


namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(Slider))]
    public class SliderEditor : Editor
    {
        public AssetSettings settings;

        Slider myTarget;
        SerializedObject soTarget;

        SerializedProperty autoFocusOnGameStart;
        SerializedProperty interactable;

        SerializedProperty minValue;
        SerializedProperty maxValue;
        SerializedProperty handle;
        SerializedProperty background;
        SerializedProperty backgroundSize;

        SerializedProperty keyStep;

        SerializedProperty handleGraphic;
        SerializedProperty progressBar;
        SerializedProperty selectedHandleMat;
        SerializedProperty unSelectedHandleMat;
        SerializedProperty clickedHandleMat;
        SerializedProperty disabledHandleMat;

        SerializedProperty useEvents;
        SerializedProperty onValueChanged;
        SerializedProperty sliderDragEnded;

        SerializedProperty useValueRangeEvents;
        SerializedProperty valueRangeEvents;

        float value;

        AnimBool showMainSettings;
        AnimBool showVisualSettings;
        AnimBool showKeyboardSettings;
        AnimBool showEventsSettings;
        AnimBool showValueRangeSettings;

        Texture documentationIcon;
        Texture addIcon;
        Texture deleteIcon;
        readonly float iconSize = 20;

        readonly string[] directionOptions = new[] { "Left to Right", "Right to Left" };

        GUIStyle toggleStyle;
        GUIStyle foldOutStyle;
        GUIStyle iconButtonStyle = null;
        GUIStyle defaultLabel = null;
        GUIStyle defaultMultilineLabel = null;
        GUIStyle myStyleButton;

        static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 1);
        static readonly Color toggledOnButtonColor = Color.yellow;
        static readonly Color toggledOffButtonColor = Color.white;

        readonly string variableName = "\n\nVariable name: ";

        Type xrToolkitSetupClass;

        void OnEnable()
        {
            myTarget = (Slider)target;
            soTarget = new SerializedObject(target);

            documentationIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Documentation.png") as Texture;
            addIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Plus.png") as Texture;
            deleteIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Cross.png") as Texture;

            xrToolkitSetupClass = Type.GetType("TinyGiantStudio.Text.XRToolkitEditorSetup");

            FindProperties();

            AnimBools();

            if (!settings)
                settings = StaticMethods.VerifySettings(settings);
        }

        public override void OnInspectorGUI()
        {
            GenerateStyle();
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            MainSettings();
            GUILayout.Space(10);
            VisualSettings();
            SliderInputSettings();
            EventSettings();

            if (xrToolkitSetupClass != null)
                xrToolkitSetupClass.GetMethod("CreateSetupButtonForSlider").Invoke(null, new object[] { myTarget.gameObject, myStyleButton });

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                ApplyModifiedValuesToHandleAndBackground();
                EditorUtility.SetDirty(myTarget);
            }
        }


        void MainSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUIContent content = new GUIContent("Main");
            showMainSettings.target = EditorGUILayout.Foldout(showMainSettings.target, content, true, foldOutStyle);
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showMainSettings.faded))
            {
                EditorGUI.indentLevel = 1;
                float defaultLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 90;
                if (!StaticMethods.GetParentList(myTarget.transform))
                {
                    EditorGUILayout.PropertyField(autoFocusOnGameStart, new GUIContent("Auto Focus", "If set to true, the slider is focused on awake. The slider is scrollable with a keyboard when focused.If it is in a list, the list controls who to focus on." + variableName + "autoFocusOnGameStart"));
                }
                EditorGUILayout.PropertyField(interactable, new GUIContent("Interactable", "As the name suggests, if it isn't interactable nothing can interact with it and it gets ignored in a list" + variableName + "interactable"));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = defaultLabelWidth;


                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 85;
                EditorGUILayout.PropertyField(minValue, new GUIContent("Min Value", "The minimum value allowed by the slider." + variableName + "minValue"), GUILayout.MinWidth(100));
                EditorGUIUtility.labelWidth = 85;
                EditorGUILayout.PropertyField(maxValue, new GUIContent("Max Value", "The maximum value allowed by the slider." + variableName + "maxValue"), GUILayout.MinWidth(110));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Current Value", GUILayout.MaxWidth(100));

                Undo.RecordObject(myTarget, "Update slider"); //todo: what if nothing is changed                
                value = EditorGUILayout.Slider(myTarget.CurrentValue, myTarget.minValue, myTarget.maxValue);

                GUILayout.EndHorizontal();

                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        void VisualSettings()
        {
            float defaultLabelWidth = EditorGUIUtility.labelWidth;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUIContent content = new GUIContent("Visual", variableName + "directionChoice\nfor choosing the direction of the slider.");
            GUILayout.BeginHorizontal();
            showVisualSettings.target = EditorGUILayout.Foldout(showVisualSettings.target, content, true, foldOutStyle);
            Documentation("https://ferdowsur.gitbook.io/modular-3d-text/utility/ui-states", "Understanding UI States");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showVisualSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                EditorGUIUtility.labelWidth = 120;
                myTarget.directionChoice = EditorGUILayout.Popup(myTarget.directionChoice, directionOptions);

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.ItalicHorizontalField(background, "Background", variableName + "background", FieldSize.normal);
                MText_Editor_Methods.ItalicHorizontalField(backgroundSize, "Size", variableName + "backgroundSize", FieldSize.tiny);
                GUILayout.EndHorizontal();

                MText_Editor_Methods.ItalicHorizontalField(progressBar, "Progress Bar", variableName + "progressBar", FieldSize.normal);

                Handle();

                EditorGUIUtility.labelWidth = defaultLabelWidth;
                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        void Handle()
        {
            EditorGUI.indentLevel = 0;
            GUILayout.Space(5);
            GUILayout.Label("Handle", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            MText_Editor_Methods.ItalicHorizontalField(handle, "SliderHandle", "Gameobject with the component 'SliderHandle' must be assigned here." + variableName + "handle", FieldSize.normal);
            MText_Editor_Methods.ItalicHorizontalField(handleGraphic, "Graphic", "The Renderer whose material will be changed according to state needs to be assigned here." + variableName + "handleGraphic", FieldSize.normal);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            MText_Editor_Methods.PreviewField(unSelectedHandleMat, myTarget.unSelectedHandleMat, "unSelected", variableName + "unSelectedHandleMat");
            MText_Editor_Methods.PreviewField(selectedHandleMat, myTarget.selectedHandleMat, "Selected", variableName + "selectedHandleMat");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            MText_Editor_Methods.PreviewField(clickedHandleMat, myTarget.clickedHandleMat, "Clicked", variableName + "clickedHandleMat");
            MText_Editor_Methods.PreviewField(disabledHandleMat, myTarget.disabledHandleMat, "Disabled", variableName + "disabledHandleMat");
            GUILayout.EndHorizontal();
        }

        void SliderInputSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUIContent content = new GUIContent("Input");
            GUILayout.BeginHorizontal();
            showKeyboardSettings.target = EditorGUILayout.Foldout(showKeyboardSettings.target, content, true, foldOutStyle);
            Documentation("https://ferdowsur.gitbook.io/modular-3d-text/ui/slider#input", "How is input Handled?");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showKeyboardSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                EditorGUILayout.PropertyField(keyStep, new GUIContent("Key Step", "Controls how fast the slider moves on input." + variableName + "keyStep"));

                string buttonText = "Add input system";
                buttonText = UpdateWarnings(buttonText);

                if (GUILayout.Button(new GUIContent(buttonText, "This adds InputSystemController controller scripts and updates the methods.")))
                {
                    SetupInputSystem();
                }
#if ENABLE_INPUT_SYSTEM
                if (myTarget.gameObject.GetComponent<PlayerInput>())
                    if (!myTarget.gameObject.GetComponent<PlayerInput>().actions)
                        EditorGUILayout.HelpBox("Couldnt find InputActionAsset. Please attach 3D Text UI Control.", MessageType.Info);
#endif
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
            GUILayout.Space(10);
        }

        /// <summary>
        /// Similar code exists in list editor
        /// </summary>
        private void SetupInputSystem()
        {
            if (!myTarget.gameObject.GetComponent<ButtonInputSystemLocal>())
                Undo.AddComponent(myTarget.gameObject, typeof(ButtonInputSystemLocal));

            SetupLocalButtonInputSystem();
            SetupButtonInputProcessor();


#if ENABLE_INPUT_SYSTEM
#if UNITY_2023_1_OR_NEWER
            ButtonInputSystemGlobal buttonInputSystemGlobal = (ButtonInputSystemGlobal)UnityEngine.Object.FindFirstObjectByType(typeof(ButtonInputSystemGlobal));
#else
            ButtonInputSystemGlobal buttonInputSystemGlobal = (ButtonInputSystemGlobal)GameObject.FindObjectOfType(typeof(ButtonInputSystemGlobal));
#endif

            bool usePlayerInput = true;
            if (buttonInputSystemGlobal)
            {
                if (buttonInputSystemGlobal.MyButtonInputProcessorStyle == ButtonInputSystemGlobal.ButtonInputProcessorStyle.CommonInputController)
                    usePlayerInput = false;
            }

            if (usePlayerInput)
            {
                if (!myTarget.gameObject.GetComponent<PlayerInput>())
                    myTarget.gameObject.AddComponent<PlayerInput>().neverAutoSwitchControlSchemes = true;

                if (settings)
                    myTarget.gameObject.GetComponent<PlayerInput>().actions = settings.InputActionAsset;
            }
            else
            {
                if (myTarget.gameObject.GetComponent<PlayerInput>())
                    DestroyImmediate(myTarget.gameObject.GetComponent<PlayerInput>());

            }
#endif
        }

        /// <summary>
        /// Similar code exists in list editor
        /// </summary>
        private void SetupLocalButtonInputSystem()
        {
            Undo.RecordObject(myTarget.gameObject.GetComponent<ButtonInputSystemLocal>(), "Update slider");

            ButtonInputSystemLocal buttonInputSystemLocal = myTarget.gameObject.GetComponent<ButtonInputSystemLocal>();
            Slider slider = myTarget.gameObject.GetComponent<Slider>();

            if (buttonInputSystemLocal.leftAxisEvent == null)
                buttonInputSystemLocal.leftAxisEvent = new UnityEvent();
            if (!CheckIfContains(myTarget.gameObject.GetComponent<ButtonInputSystemLocal>().leftAxisEvent, myTarget.gameObject.GetComponent<Slider>(), "DecreaseValue"))
                UnityEventTools.AddPersistentListener(buttonInputSystemLocal.leftAxisEvent, slider.DecreaseValue);


            if (buttonInputSystemLocal.rightAxisEvent == null)
                buttonInputSystemLocal.rightAxisEvent = new UnityEvent();
            if (!CheckIfContains(myTarget.gameObject.GetComponent<ButtonInputSystemLocal>().rightAxisEvent, myTarget.gameObject.GetComponent<Slider>(), "IncreaseValue"))
                UnityEventTools.AddPersistentListener(buttonInputSystemLocal.rightAxisEvent, slider.IncreaseValue);
        }

        void SetupButtonInputProcessor()
        {
            ButtonInputSystemLocal buttonInputSystemLocal = myTarget.gameObject.GetComponent<ButtonInputSystemLocal>();

            if (!myTarget.gameObject.GetComponent<ButtonInputProcessor>())
                Undo.AddComponent(myTarget.gameObject, typeof(ButtonInputProcessor));

            ButtonInputProcessor buttonInputProcessor = myTarget.GetComponent<ButtonInputProcessor>();

            if (buttonInputProcessor == null)
                return;


            if (buttonInputProcessor.upAxisEvent == null)
                buttonInputProcessor.upAxisEvent = new UnityEvent();
            if (!CheckIfContains(buttonInputProcessor.upAxisEvent, this, "UpAxisEvent"))
                UnityEventTools.AddPersistentListener(buttonInputProcessor.upAxisEvent, buttonInputSystemLocal.UpAxisEvent);

            if (buttonInputProcessor.downAxisEvent == null)
                buttonInputProcessor.downAxisEvent = new UnityEvent();
            if (!CheckIfContains(buttonInputProcessor.downAxisEvent, this, "DownAxisEvent"))
                UnityEventTools.AddPersistentListener(buttonInputProcessor.downAxisEvent, buttonInputSystemLocal.DownAxisEvent);

            if (buttonInputProcessor.leftAxisEvent == null)
                buttonInputProcessor.leftAxisEvent = new UnityEvent();
            if (!CheckIfContains(buttonInputProcessor.leftAxisEvent, this, "LeftAxisEvent"))
                UnityEventTools.AddPersistentListener(buttonInputProcessor.leftAxisEvent, buttonInputSystemLocal.LeftAxisEvent);

            if (buttonInputProcessor.rightAxisEvent == null)
                buttonInputProcessor.rightAxisEvent = new UnityEvent();
            if (!CheckIfContains(buttonInputProcessor.rightAxisEvent, this, "RightAxisEvent"))
                UnityEventTools.AddPersistentListener(buttonInputProcessor.rightAxisEvent, buttonInputSystemLocal.RightAxisEvent);

            if (buttonInputProcessor.submitEvent == null)
                buttonInputProcessor.submitEvent = new UnityEvent();
            if (!CheckIfContains(buttonInputProcessor.submitEvent, this, "SubmitEvent"))
                UnityEventTools.AddPersistentListener(buttonInputProcessor.submitEvent, buttonInputSystemLocal.SubmitEvent);

        }
        private string UpdateWarnings(string buttonText)
        {
            string notSetup = "No Input system script attached. It is completely fine if intended, otherwise please add it to control the list via keyboard/similar devices.";

            if (!myTarget.gameObject.GetComponent<ButtonInputSystemLocal>())
            {
                EditorGUILayout.HelpBox(notSetup, MessageType.Info);
                return "Add input system";
            }



#if ENABLE_INPUT_SYSTEM
#if UNITY_2023_1_OR_NEWER
            ButtonInputSystemGlobal buttonInputSystemGlobal = (ButtonInputSystemGlobal)UnityEngine.Object.FindFirstObjectByType(typeof(ButtonInputSystemGlobal));
#else
            ButtonInputSystemGlobal buttonInputSystemGlobal = (ButtonInputSystemGlobal)GameObject.FindObjectOfType(typeof(ButtonInputSystemGlobal));
#endif //of unity version check

            bool usePlayerInput = false;
            if (buttonInputSystemGlobal != null)
            {
                if (buttonInputSystemGlobal.MyButtonInputProcessorStyle == ButtonInputSystemGlobal.ButtonInputProcessorStyle.IndividualPlayerInputComponents)
                    usePlayerInput = true;
            }
            else
                usePlayerInput = true;

            if (myTarget.gameObject.GetComponent<PlayerInput>())
            {
                if (!usePlayerInput)
                {
                    EditorGUILayout.HelpBox(
                        "Player input component is attached while the common control is turned on. " +
                        "Please remove the player input component or change the control type to individual controls from the Input Processor script."
                        , MessageType.Warning);
                }
            }
            else
            {
                if (usePlayerInput)
                {
                    if (Application.isPlaying)
                        EditorGUILayout.HelpBox("No PlayerInput script attached. It is completely fine if intended, otherwise please add it to control the list via keyboard/similar devices.", MessageType.Info);
                    else
                        EditorGUILayout.HelpBox("No PlayerInput script attached. It is completely fine if intended, otherwise please add it to control the list via keyboard/similar devices.", MessageType.Info);
                }
                else
                    buttonText = "Update input system";
            }
#endif

            return "Update input system";
        }



        void EventSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useEvents, GUIContent.none, GUILayout.MaxWidth(25));
            showEventsSettings.target = EditorGUILayout.Foldout(showEventsSettings.target, new GUIContent("Events", ""), true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showEventsSettings.faded))
            {
                EditorGUILayout.PropertyField(onValueChanged);
                EditorGUILayout.PropertyField(sliderDragEnded);

                GUILayout.Space(5);
                ValueRanges();
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        void ValueRanges()
        {
            GUILayout.Space(10);
            GUIContent tabName = new GUIContent("Value Range Events", "Events are called when the slider value enters a specific range.\nChecks value in the top to down order. If you have two ranges that can be fulfilled simultaneously, the first one gets called.");

            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.toolbar);

            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useValueRangeEvents, GUIContent.none, GUILayout.MaxWidth(25));

            showValueRangeSettings.target = EditorGUILayout.Foldout(showValueRangeSettings.target, tabName, true, foldOutStyle);
            //GUILayout.FlexibleSpace();
            GUILayout.Label(myTarget.valueRangeEvents.Count.ToString(), GUILayout.Width(20));
            if (GUILayout.Button(addIcon, iconButtonStyle, GUILayout.MaxHeight(iconSize), GUILayout.MaxWidth(iconSize)))
            {
                myTarget.NewValueRange();
                EditorUtility.SetDirty(target);

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndVertical();

                return;
            }
            Documentation("https://ferdowsur.gitbook.io/modular-3d-text/slider#value-range-events", "Value range events");

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showValueRangeSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                float defaultLabelWidth = EditorGUIUtility.labelWidth;

                GUILayout.Space(5);
                ValuesAreIn();

                for (int i = 0; i < myTarget.valueRangeEvents.Count; i++)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel = 0;
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    GUILayout.Label("Event : " + (i + 1), EditorStyles.boldLabel);
                    if (GUILayout.Button(deleteIcon, iconButtonStyle, GUILayout.MaxHeight(iconSize), GUILayout.MaxWidth(iconSize)))
                    {
                        myTarget.valueRangeEvents.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        return;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal();
                    string minmaxTooltip = "Events get triggered when within this min and max range";
                    EditorGUIUtility.labelWidth = 60;
                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("min"), new GUIContent("Min Value", minmaxTooltip), GUILayout.MinWidth(90));
                    EditorGUIUtility.labelWidth = 65;
                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("max"), new GUIContent("Max Value", minmaxTooltip), GUILayout.MinWidth(90));
                    GUILayout.EndHorizontal();
                    EditorGUIUtility.labelWidth = defaultLabelWidth;

                    if (myTarget.usePercentage && (myTarget.valueRangeEvents[i].min > 100 || myTarget.valueRangeEvents[i].max > 100))
                        EditorGUILayout.HelpBox("A range value is greater than 100 percant. Uncheck Use Percentage if you want to use direct values", MessageType.Warning);

                    if (myTarget.valueRangeEvents[i].min > myTarget.valueRangeEvents[i].max)
                        EditorGUILayout.HelpBox("Minimum value is greater than maximum", MessageType.Warning);


                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("icon"));
                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("oneTimeEvents"));
                    EditorGUILayout.PropertyField(valueRangeEvents.GetArrayElementAtIndex(i).FindPropertyRelative("repeatEvents"), new GUIContent("On change", "Gets called everytime slider value is changed at this range"));


                    GUILayout.EndVertical();
                }
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                GUILayout.Label(GUIContent.none, GUILayout.MaxWidth(2));
                if (GUILayout.Button(addIcon, iconButtonStyle, GUILayout.MaxHeight(iconSize), GUILayout.MinHeight(30)))
                {
                    myTarget.NewValueRange();
                }
                GUILayout.Label(GUIContent.none, GUILayout.MaxWidth(2));
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        void ValuesAreIn()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Values are in", defaultLabel, GUILayout.MaxWidth(65));

            Color original = GUI.backgroundColor;

            if (myTarget.usePercentage)
                GUI.backgroundColor = toggledOnButtonColor;
            else
                GUI.backgroundColor = toggledOffButtonColor;

            GUIContent usePercentage = new GUIContent("Percentage", "Values in value range events are in percentage.");
            if (LeftButton(usePercentage))
            {
                myTarget.usePercentage = true;
                EditorUtility.SetDirty(myTarget);
            }


            if (!myTarget.usePercentage)
                GUI.backgroundColor = toggledOnButtonColor;
            else
                GUI.backgroundColor = toggledOffButtonColor;

            GUIContent capitalize = new GUIContent("Value", "alues in value range events are directly stated.");
            if (RightButton(capitalize))
            {
                myTarget.usePercentage = false;
                EditorUtility.SetDirty(myTarget);
            }

            GUI.backgroundColor = original;
            EditorGUILayout.EndHorizontal();
        }

        bool LeftButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);

            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(0, 0, rect.width + toggleStyle.border.right, rect.height), content, toggleStyle))
                clicked = true;

            GUI.EndGroup();
            return clicked;
        }
        //bool MidButton(GUIContent content)
        //{
        //    bool clicked = false;
        //    Rect rect = GUILayoutUtility.GetRect(20, 20);


        //    GUI.BeginGroup(rect);
        //    if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left + toggleStyle.border.right, rect.height), content, toggleStyle))
        //        //if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left + toggleStyle.border.right, rect.height), content, toggleStyle))
        //        clicked = true;
        //    GUI.EndGroup();
        //    return clicked;
        //}
        bool RightButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);

            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left, rect.height), content, toggleStyle))
                clicked = true;
            GUI.EndGroup();
            return clicked;
        }

        bool CheckIfContains(UnityEvent myEvent, object target, string targetMethodName)
        {
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

        void Documentation(string URL, string subject)
        {
            GUIContent doc = new GUIContent(documentationIcon, subject + " documentation\n\nURL: " + URL);
            if (GUILayout.Button(doc, iconButtonStyle, GUILayout.Height(iconSize), GUILayout.Width(iconSize)))
            {
                Application.OpenURL(URL);
            }
        }

        void ApplyModifiedValuesToHandleAndBackground()
        {
            if (value != myTarget.CurrentValue)
                myTarget.CurrentValue = value;

            if (myTarget.background)
                myTarget.UpdateBackgroundSize();
            //myTarget.background.localScale = new Vector3(myTarget.backgroundSize, myTarget.background.localScale.y, myTarget.background.localScale.z);

            if (myTarget)
                myTarget.UpdateGraphic();

            if (myTarget.interactable)
                myTarget.UnSelectedVisual();
            else
                myTarget.DisabledVisual();
        }

        void FindProperties()
        {
            autoFocusOnGameStart = soTarget.FindProperty("autoFocusOnGameStart");
            interactable = soTarget.FindProperty("interactable");

            minValue = soTarget.FindProperty("minValue");
            maxValue = soTarget.FindProperty("maxValue");

            handle = soTarget.FindProperty("handle");
            background = soTarget.FindProperty("background");
            backgroundSize = soTarget.FindProperty("backgroundSize");

            keyStep = soTarget.FindProperty("keyStep");

            onValueChanged = soTarget.FindProperty("onValueChanged");
            sliderDragEnded = soTarget.FindProperty("sliderDragEnded");
            useEvents = soTarget.FindProperty("useEvents");

            handleGraphic = soTarget.FindProperty("handleGraphic");
            progressBar = soTarget.FindProperty("progressBar");
            selectedHandleMat = soTarget.FindProperty("selectedHandleMat");
            unSelectedHandleMat = soTarget.FindProperty("unSelectedHandleMat");
            clickedHandleMat = soTarget.FindProperty("clickedHandleMat");
            disabledHandleMat = soTarget.FindProperty("disabledHandleMat");

            useValueRangeEvents = soTarget.FindProperty("useValueRangeEvents");
            valueRangeEvents = soTarget.FindProperty("valueRangeEvents");
        }

        void AnimBools()
        {
            showMainSettings = new AnimBool(true);
            showMainSettings.valueChanged.AddListener(Repaint);

            showVisualSettings = new AnimBool(false);
            showVisualSettings.valueChanged.AddListener(Repaint);

            showKeyboardSettings = new AnimBool(false);
            showKeyboardSettings.valueChanged.AddListener(Repaint);

            showEventsSettings = new AnimBool(false);
            showEventsSettings.valueChanged.AddListener(Repaint);

            showValueRangeSettings = new AnimBool(false);
            showValueRangeSettings.valueChanged.AddListener(Repaint);
        }

        void GenerateStyle()
        {
            if (EditorGUIUtility.isProSkin)
            {
                if (settings)
                    openedFoldoutTitleColor = settings.openedFoldoutTitleColor_darkSkin;
            }
            else
            {
                if (settings)
                    openedFoldoutTitleColor = settings.openedFoldoutTitleColor_lightSkin;
            }



            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    overflow = new RectOffset(-10, 0, 3, 0),
                    padding = new RectOffset(15, 0, -3, 0),
                    fontStyle = FontStyle.Bold
                };
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }

            iconButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);

            if (toggleStyle == null)
            {
                toggleStyle = new GUIStyle(GUI.skin.button);
                toggleStyle.margin = new RectOffset(0, 0, toggleStyle.margin.top, toggleStyle.margin.bottom);
                toggleStyle.fontStyle = FontStyle.Bold;
                toggleStyle.active.textColor = Color.yellow;
            }

            if (defaultLabel == null)
            {
                defaultLabel = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 9
                };
                defaultLabel.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
            }

            if (defaultMultilineLabel == null)
            {
                defaultMultilineLabel = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 10,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter,
                };
                defaultMultilineLabel.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
            }

            if (myStyleButton == null)
            {
                myStyleButton = new GUIStyle("Button")
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 12
                };
                if (EditorGUIUtility.isProSkin)
                {
                    myStyleButton.normal.textColor = Color.white;
                    myStyleButton.hover.textColor = Color.yellow;
                    myStyleButton.active.textColor = Color.green;
                }
                else
                {
                    myStyleButton.normal.textColor = Color.black;
                    myStyleButton.hover.textColor = Color.red;
                    myStyleButton.active.textColor = Color.black;
                }
            }
        }
    }
}