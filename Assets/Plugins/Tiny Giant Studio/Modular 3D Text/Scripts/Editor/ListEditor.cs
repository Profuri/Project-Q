using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using TinyGiantStudio.Modules;
using TinyGiantStudio.EditorHelpers;


namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(List))]
    public class ListEditor : Editor
    {
        public AssetSettings settings;

        List myTarget;
        SerializedObject soTarget;

        SerializedProperty autoFocusOnStart;
        SerializedProperty autoFocusFirstItem;

        SerializedProperty UseStyle;

        SerializedProperty useNormalItemVisual;
        SerializedProperty normalTextSize;
        SerializedProperty normalItemTextMaterial;
        SerializedProperty normalItemBackgroundMaterial;

        SerializedProperty useSelectedItemVisual;
        SerializedProperty selectedItemFontSize;
        SerializedProperty selectedItemTextMaterial;
        SerializedProperty selectedItemBackgroundMaterial;

        SerializedProperty usePressedItemVisual;
        SerializedProperty pressedItemFontSize;
        SerializedProperty pressedItemTextMaterial;
        SerializedProperty pressedItemBackgroundMaterial;
        SerializedProperty holdPressedVisualFor;

        SerializedProperty useDisabledItemVisual;
        SerializedProperty disabledItemFontSize;
        SerializedProperty disabledItemTextMaterial;
        SerializedProperty disabledItemBackgroundMaterial;


        SerializedProperty useModules;
        SerializedProperty ignoreChildModules;

        SerializedProperty ignoreChildUnSelectModuleContainers;
        SerializedProperty applyUnSelectModuleContainers;
        SerializedProperty unSelectModuleContainers;

        SerializedProperty ignoreChildSelectedModuleContainers;
        SerializedProperty applySelectedModuleContainers;
        SerializedProperty onSelectModuleContainers;

        SerializedProperty ignoreChildBeingPressedModuleContainers;
        SerializedProperty applyBeingPressedModuleContainers;
        SerializedProperty beingPressedModuleContainers;

        SerializedProperty ignoreChildPressCompleteModuleContainers;
        SerializedProperty applyPressCompleteModuleContainers;
        SerializedProperty pressCompleteModuleContainers;

        SerializedProperty switchCurrentControlSchemeInPlayerInputWhenFocused;

        SerializedProperty selectedItem;


        //private static Color toggledOnButtonColor = Color.white;
        static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 1);
        static Color toggledOffColor = new Color(0.75f, 0.75f, 0.75f);
        readonly string toggleDescription = "The toggle controls if the style should be applied from list. If disabled, the individual list elements will control their own styles.";
        readonly string variableName = "\n\nVariable name: ";


        GUIStyle iconButtonStyle = null;
        GUIStyle foldOutStyle = null;
        GUIStyle defaultLabelStyle = null;
        GUIStyle headerLabel = null;

        AnimBool showLayoutSettingsInEditor;
        AnimBool showModuleSettingsInEditor;
        AnimBool showDebugSettingsInEditor;
        AnimBool showDisabledItemSettings;
        AnimBool showPressedItemSettings;
        AnimBool showSelectedItemSettings;
        AnimBool showNormalItemSettings;
        AnimBool showChildVisualSettings;

        AnimBool showInputSettings;

        Texture documentationIcon;
        //Texture addIcon;
        //Texture deleteIcon;
        readonly float iconSize = 20;
        readonly int spaceAtTheBottomOfABox = 6;

        void Awake()
        {
            Undo.undoRedoPerformed += UndoRedo;
        }
        void OnEnable()
        {
            myTarget = (List)target;
            soTarget = new SerializedObject(target);

            GetReferences();

            if (!settings)
                settings = StaticMethods.VerifySettings(settings);

            documentationIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Documentation.png") as Texture;
            //addIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Plus.png") as Texture;
            //deleteIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Cross.png") as Texture;
        }
        public override void OnInspectorGUI()
        {
            GenerateStyle();
            soTarget.Update();
            EditorGUI.BeginChangeCheck();
            MainSettings();

            GUILayout.Space(10);
            VisualSettings();
            GUILayout.Space(5);
            ModuleSettings();
            GUILayout.Space(5);
            InputSettings();
            GUILayout.Space(5);
            DebugSettings();

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();

                if (!EditorApplication.isPlaying)
                    myTarget.UnselectEverything();
                else
                    myTarget.UpdateStyle();

                EditorUtility.SetDirty(myTarget);
            }
        }



        void MainSettings()
        {
            EditorGUI.indentLevel = 0;

            AutoSelectionSettings();
        }
        void VisualSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(UseStyle, GUIContent.none, GUILayout.MaxWidth(25));
            GUIContent showVisualSettingsContent = new GUIContent("Style", "Styles control child element visuals. If set to false, disables all style controls from this list. \n\nThe name of the bool: 'UseStyles'");
            showChildVisualSettings.target = EditorGUILayout.Foldout(showChildVisualSettings.target, showVisualSettingsContent, true, foldOutStyle);
            Documentation("https://ferdowsur.gitbook.io/modular-3d-text/utility/ui-states", "Understanding UI States");
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 1;
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showChildVisualSettings.faded))
            {
                GUILayout.Space(5);
                NormalItemSettings();
                GUILayout.Space(2.5f);
                SelectedItemSettings();
                GUILayout.Space(2.5f);
                PressedItemSettings();
                GUILayout.Space(2.5f);
                DisabledItemSettings();

                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }



        void ModuleSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(useModules, GUIContent.none, GUILayout.MaxWidth(25));
            showModuleSettingsInEditor.target = EditorGUILayout.Foldout(showModuleSettingsInEditor.target, new GUIContent("Modules", "If set to false, disables all modules from this list. Modules are called when entering selected style for the first time." + variableName + "useModules"), true, foldOutStyle);
            Documentation("https://ferdowsur.gitbook.io/modules/", "Modules");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();


            if (EditorGUILayout.BeginFadeGroup(showModuleSettingsInEditor.faded))
            {
                Color contentDefaultColor = GUI.contentColor;
                if (!myTarget.useModules)
                    GUI.contentColor = toggledOffColor;

                EditorGUI.indentLevel = 1;
                GUILayout.Space(2);
                MText_Editor_Methods.ItalicHorizontalField(ignoreChildModules, "Ignore All Child Modules", "If set to false, disables all modules on the list items like buttons." + variableName + "ignoreChildModules", FieldSize.gigantic);
                GUILayout.Space(10);

                OnUnselectModules();

                GUILayout.Space(10);

                SelectdModules();

                GUILayout.Space(10);

                BeingPressedModules();

                GUILayout.Space(10);
                PressCompleteModules();


                GUI.contentColor = contentDefaultColor;
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        readonly string unSelectMeans = "Unselect is the state that appears when a UI element enters the normal state from selected.";
        void OnUnselectModules()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            string tooltip_onUnSelect = unSelectMeans + variableName + "applyUnSelectedModuleContainers for \ndisabling/enabling." + variableName + "unSelectedModuleContainers\n for the module container.";
            MText_Editor_Methods.ItalicHorizontalField(ignoreChildUnSelectModuleContainers, "Ignore Child UnSelect Modules", "If Ignore Child UnSelect Modules is set to true, if any child element like buttons have Un-Select Modules, they are ignored.\n" + unSelectMeans + variableName + "ignoreChildUnSelectedModuleContainers", FieldSize.gigantic);
            ModuleDrawer.BaseModuleContainerList("UnSelected", tooltip_onUnSelect, myTarget.unSelectedModuleContainers, unSelectModuleContainers, soTarget, applyUnSelectModuleContainers);
            GUILayout.EndVertical();
        }

        readonly string selectMeans = "Selected is the state when Mouse hovers on a UI item or it is selected via keyboard/controller by scrolling in a list.";
        void SelectdModules()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            string tooltip_onSelect = selectMeans + variableName + "applySelectedModuleContainers \nfor disabling/enabling." + variableName + "selectedModuleContainers\n for the module container.";
            MText_Editor_Methods.ItalicHorizontalField(ignoreChildSelectedModuleContainers, "Ignore Child Selected Modules", "If Ignore Child Selected Modules is set to true, if any child element like buttons have selected modules, they are ignored.\n" + unSelectMeans + variableName + "ignoreChildSelectedModuleContainers", FieldSize.gigantic);
            ModuleDrawer.BaseModuleContainerList("Selected", tooltip_onSelect, myTarget.selectedModuleContainers, onSelectModuleContainers, soTarget, applySelectedModuleContainers);
            GUILayout.EndVertical();
        }

        readonly string beingPressedMeans = "While the mouse click or touch is held down, the module is constantly called.";
        void BeingPressedModules()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            string tooltip_whileBeingPressed = beingPressedMeans + variableName + "applyBeingPressedModuleContainers \nfor disabling/enabling" + variableName + "beingPressedModuleContainers\n for the module container.";
            MText_Editor_Methods.ItalicHorizontalField(ignoreChildBeingPressedModuleContainers, "Ignore Child Being Pressed Modules", "If Ignore Child Being Pressed Modules is set to true, if any child element like buttons have being Pressed Modules, they are ignored. \n" + beingPressedMeans + variableName + "ignoreChildBeingPressedModuleContainers", FieldSize.gigantic);
            ModuleDrawer.BaseModuleContainerList("Being Pressed", tooltip_whileBeingPressed, myTarget.beingPressedModuleContainers, beingPressedModuleContainers, soTarget, applyBeingPressedModuleContainers);
            GUILayout.EndVertical();
        }


        readonly string pressCompleteMeans = "When the user releases the key.";
        void PressCompleteModules()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            string tooltip_onClick = pressCompleteMeans + variableName + "applyPressCompleteModuleContainers \nfor disabling/enabling" + variableName + "pressCompleteModuleContainers\n for the module container.";
            MText_Editor_Methods.ItalicHorizontalField(ignoreChildPressCompleteModuleContainers, "Ignore Child Press Complete Modules", "If Ignore Child Press Complete Modules is set to true, if any child element like buttons have Press Complete modules, they are ignored.", FieldSize.gigantic);
            ModuleDrawer.BaseModuleContainerList("Press Complete", tooltip_onClick, myTarget.pressCompleteModuleContainers, pressCompleteModuleContainers, soTarget, applyPressCompleteModuleContainers);
            GUILayout.EndVertical();
        }






        void DebugSettings()
        {
            if (!Application.isPlaying)
                return;


            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent showDebugContent = new GUIContent("Debug", "");
            showDebugSettingsInEditor.target = EditorGUILayout.Foldout(showDebugSettingsInEditor.target, showDebugContent, true, foldOutStyle);
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 1;
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDebugSettingsInEditor.faded))
            {
                if (myTarget.selected)
                    GUILayout.Label("Selected", headerLabel);
                else
                    GUILayout.Label("Not selected", headerLabel);

                GUILayout.Space(5);
                EditorGUILayout.PropertyField(selectedItem);
                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        void AutoSelectionSettings()
        {
            MText_Editor_Methods.ItalicHorizontalField(autoFocusOnStart, "Auto Focus List", "If set to true, the list is focused on awake. The list is scrollable with a keyboard when focused.\n\nVariable name: 'autoFocusOnStart'", FieldSize.gigantic);
            MText_Editor_Methods.ItalicHorizontalField(autoFocusFirstItem, "Auto Focus first item in List", "Selects first item when focused.\n\nVariable name: 'autoFocusFirstItem'", FieldSize.gigantic);
        }

        void NormalItemSettings()
        {
            Color contentDefaultColor = GUI.contentColor;
            if (!myTarget.UseStyle || !myTarget.UseNormalItemVisual)
                GUI.contentColor = toggledOffColor;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useNormalItemVisual, GUIContent.none, GUILayout.MaxWidth(25));
            showNormalItemSettings.target = EditorGUILayout.Foldout(showNormalItemSettings.target, new GUIContent("Normal", toggleDescription + variableName + "UseNormalItemVisual"), true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showNormalItemSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Text Size", variableName + "NormalTextSize"), defaultLabelStyle, GUILayout.MaxWidth(70));
                EditorGUILayout.PropertyField(normalTextSize, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.PreviewField(normalItemTextMaterial, myTarget.NormalTextMaterial, "Text", variableName + "NormalTextMaterial");
                MText_Editor_Methods.PreviewField(normalItemBackgroundMaterial, myTarget.NormalBackgroundMaterial, "Background", variableName + "NormalBackgroundMaterial");
                GUILayout.EndHorizontal();

                GUILayout.Space(spaceAtTheBottomOfABox);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();

            GUI.contentColor = contentDefaultColor;
        }
        void SelectedItemSettings()
        {
            Color contentDefaultColor = GUI.contentColor;
            if (!myTarget.UseStyle || !myTarget.UseSelectedItemVisual)
                GUI.contentColor = toggledOffColor;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useSelectedItemVisual, GUIContent.none, GUILayout.MaxWidth(25));
            GUIContent content = new GUIContent("Selected", "Mouse hover or selected in a list ready to be clicked.\n\n" + toggleDescription + variableName + "UseSelectedItemVisual");
            showSelectedItemSettings.target = EditorGUILayout.Foldout(showSelectedItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showSelectedItemSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Text Size", variableName + "SelectedTextSize"), defaultLabelStyle, GUILayout.MaxWidth(70));
                EditorGUILayout.PropertyField(selectedItemFontSize, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.PreviewField(selectedItemTextMaterial, myTarget.SelectedTextMaterial, "Text", variableName + "SelectedTextMaterial");
                MText_Editor_Methods.PreviewField(selectedItemBackgroundMaterial, myTarget.SelectedBackgroundMaterial, "Background", variableName + "SelectedBackgroundMaterial");
                GUILayout.EndHorizontal();

                GUILayout.Space(spaceAtTheBottomOfABox);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();

            GUI.contentColor = contentDefaultColor;
        }
        void PressedItemSettings()
        {
            Color contentDefaultColor = GUI.contentColor;
            if (!myTarget.UseStyle || !myTarget.UsePressedItemVisual)
                GUI.contentColor = toggledOffColor;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(usePressedItemVisual, GUIContent.none, GUILayout.MaxWidth(25));
            GUIContent content = new GUIContent("Pressed", "When click/tocuh is pressed down or for limited time after click.\n\n" + toggleDescription + variableName + "UsePressedItemVisual");
            showPressedItemSettings.target = EditorGUILayout.Foldout(showPressedItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showPressedItemSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Text Size", variableName + "PressedTextSize"), defaultLabelStyle, GUILayout.MaxWidth(70));
                EditorGUILayout.PropertyField(pressedItemFontSize, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.PreviewField(pressedItemTextMaterial, myTarget.PressedTextMaterial, "Text", variableName + "PressedTextMaterial");
                MText_Editor_Methods.PreviewField(pressedItemBackgroundMaterial, myTarget.PressedBackgroundMaterial, "Background", variableName + "PressedBackgroundMaterial");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.HorizontalField(holdPressedVisualFor, "Hold visual for", variableName + "holdPressedVisualFor", FieldSize.normal);
                GUILayout.Label("seconds", GUILayout.MaxWidth(55));
                GUILayout.EndHorizontal();

                GUILayout.Space(spaceAtTheBottomOfABox);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();

            GUI.contentColor = contentDefaultColor;
        }
        void DisabledItemSettings()
        {
            Color contentDefaultColor = GUI.contentColor;
            if (!myTarget.UseStyle || !myTarget.UseDisabledItemVisual)
                GUI.contentColor = toggledOffColor;


            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useDisabledItemVisual, GUIContent.none, GUILayout.MaxWidth(25));
            GUIContent content = new GUIContent("Disabled", "Style when button isn't interactable.\n\n" + toggleDescription + variableName + "UseDisabledItemVisual");
            showDisabledItemSettings.target = EditorGUILayout.Foldout(showDisabledItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDisabledItemSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Text Size", variableName + "DisabledTextSize"), defaultLabelStyle, GUILayout.MaxWidth(70));
                EditorGUILayout.PropertyField(disabledItemFontSize, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.PreviewField(disabledItemTextMaterial, myTarget.DisabledTextMaterial, "Text", variableName + "DisabledTextMaterial");
                MText_Editor_Methods.PreviewField(disabledItemBackgroundMaterial, myTarget.DisabledBackgroundMaterial, "Background", variableName + "DisabledBackgroundMaterial");
                GUILayout.EndHorizontal();

                GUILayout.Space(spaceAtTheBottomOfABox);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();

            GUI.contentColor = contentDefaultColor;
        }



        void UndoRedo()
        {
            if (myTarget == null)
                myTarget = (List)target;

            if (myTarget)
                myTarget.UpdateStyle();
        }


        void InputSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUIContent showInputSettingsContent = new GUIContent("Input settings", "");
            showInputSettings.target = EditorGUILayout.Foldout(showInputSettings.target, showInputSettingsContent, true, foldOutStyle);
            Documentation("https://ferdowsur.gitbook.io/modular-3d-text/input/button-key", "Input documentations");
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 1;
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showInputSettings.faded))
            {
                GUILayout.Space(5);

                string buttonText = "Add input system";
                buttonText = UpdateWarnings(buttonText);

#if ENABLE_INPUT_SYSTEM
                float width = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 500;
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(switchCurrentControlSchemeInPlayerInputWhenFocused, new GUIContent(""), GUILayout.MaxWidth(25));
                GUILayout.Label(new GUIContent(" Switch Current Control Scheme In Player Input When Focused", "If you are using the new input system, this calls the SwitchCurrentControlScheme() method in Player input, when Focus() is called."), EditorStyles.wordWrappedLabel);
                GUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = width;
#endif

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent(buttonText, "This adds InputSystemController controller scripts and updates the methods."), GUILayout.MaxHeight(30), GUILayout.MaxWidth(160)))
                {
                    SetupInputSystem();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
#if ENABLE_INPUT_SYSTEM
                if (myTarget.gameObject.GetComponent<PlayerInput>())
                    if (!myTarget.gameObject.GetComponent<PlayerInput>().actions)
                        EditorGUILayout.HelpBox("Couldnt find InputActionAsset. Please attach 3D Text UI Control from preference.", MessageType.Info);
#endif

                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        string UpdateWarnings(string buttonText)
        {
            string notSetup = "No Input system script attached. It is completely fine if intended, otherwise please add it to control the list via keyboard/similar devices.";


            if (!myTarget.gameObject.GetComponent<ButtonInputSystemLocal>())
            {
                EditorGUILayout.HelpBox(notSetup, MessageType.Info);
                return "Add input system";
            }
            else if (!myTarget.gameObject.GetComponent<ButtonInputProcessor>())
            {
                EditorGUILayout.HelpBox("No input processor script attached. Please update the input system", MessageType.Warning);
            }



#if ENABLE_INPUT_SYSTEM
#if UNITY_2023_1_OR_NEWER
            ButtonInputSystemGlobal buttonInputSystemGlobal = (ButtonInputSystemGlobal)Object.FindFirstObjectByType(typeof(ButtonInputSystemGlobal));
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
            {
                usePlayerInput = true;
            }

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
                    EditorGUILayout.HelpBox("No PlayerInput script attached. It is completely fine if intended, otherwise please add it to control the list via keyboard/similar devices.", MessageType.Info);
                else
                    buttonText = "Update input system";
            }
#endif

            return "Update input system";
        }

        void SetupInputSystem()
        {
            if (!myTarget.gameObject.GetComponent<ButtonInputSystemLocal>())
                Undo.AddComponent(myTarget.gameObject, typeof(ButtonInputSystemLocal));

            SetupLocalButtonInputSystem();
            SetupButtonInputProcessor();

#if ENABLE_INPUT_SYSTEM

#if UNITY_2023_1_OR_NEWER
            ButtonInputSystemGlobal buttonInputSystemGlobal = (ButtonInputSystemGlobal)Object.FindFirstObjectByType(typeof(ButtonInputSystemGlobal));
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
                    myTarget.gameObject.AddComponent<PlayerInput>().neverAutoSwitchControlSchemes = true; //Why was the never auto switch needed? 10/10/2023

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
        /// Similar code exists in slider editor
        /// </summary>
        void SetupLocalButtonInputSystem()
        {
            Undo.RecordObject(myTarget.gameObject.GetComponent<ButtonInputSystemLocal>(), "Update list");

            ButtonInputSystemLocal buttonInputSystemLocal = myTarget.gameObject.GetComponent<ButtonInputSystemLocal>();
            List list = myTarget.gameObject.GetComponent<List>();

            if (buttonInputSystemLocal.upAxisEvent == null)
                buttonInputSystemLocal.upAxisEvent = new UnityEvent();

            if (!CheckIfContains(buttonInputSystemLocal.upAxisEvent, list, "ScrollUp"))
                UnityEventTools.AddPersistentListener(buttonInputSystemLocal.upAxisEvent, list.ScrollUp);


            if (buttonInputSystemLocal.downAxisEvent == null)
                buttonInputSystemLocal.downAxisEvent = new UnityEvent();
            if (!CheckIfContains(buttonInputSystemLocal.downAxisEvent, list, "ScrollDown"))
                UnityEventTools.AddPersistentListener(buttonInputSystemLocal.downAxisEvent, list.ScrollDown);


            if (buttonInputSystemLocal.leftAxisEvent == null)
                buttonInputSystemLocal.leftAxisEvent = new UnityEvent();
            if (!CheckIfContains(buttonInputSystemLocal.leftAxisEvent, list, "ScrollLeft"))
                UnityEventTools.AddPersistentListener(buttonInputSystemLocal.leftAxisEvent, list.ScrollLeft);


            if (buttonInputSystemLocal.rightAxisEvent == null)
                buttonInputSystemLocal.rightAxisEvent = new UnityEvent();
            if (!CheckIfContains(buttonInputSystemLocal.rightAxisEvent, list, "ScrollRight"))
                UnityEventTools.AddPersistentListener(buttonInputSystemLocal.rightAxisEvent, list.ScrollRight);


            if (buttonInputSystemLocal.submitEvent == null)
                buttonInputSystemLocal.submitEvent = new UnityEvent();
            if (!CheckIfContains(buttonInputSystemLocal.submitEvent, list, "PressSelectedItem"))
                UnityEventTools.AddPersistentListener(buttonInputSystemLocal.submitEvent, list.PressSelectedItem);
        }
        void SetupButtonInputProcessor()
        {
            ButtonInputSystemLocal buttonInputSystemLocal = myTarget.gameObject.GetComponent<ButtonInputSystemLocal>();

            if (!myTarget.gameObject.GetComponent<ButtonInputProcessor>())
                myTarget.gameObject.AddComponent<ButtonInputProcessor>();

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






        void GetReferences()
        {
            GetAnimBoolReferences();

            autoFocusOnStart = soTarget.FindProperty("autoFocusOnStart");
            autoFocusFirstItem = soTarget.FindProperty("autoFocusFirstItem");

            switchCurrentControlSchemeInPlayerInputWhenFocused = soTarget.FindProperty("switchCurrentControlSchemeInPlayerInputWhenFocused");

            GetStyleReferences();

            GetModulesReferences();

            selectedItem = soTarget.FindProperty("selectedItem");
        }

        private void GetModulesReferences()
        {
            useModules = soTarget.FindProperty("useModules");
            ignoreChildModules = soTarget.FindProperty("ignoreChildModules");

            unSelectModuleContainers = soTarget.FindProperty("unSelectModuleContainers");
            applyUnSelectModuleContainers = soTarget.FindProperty("applyUnSelectModuleContainers");
            ignoreChildUnSelectModuleContainers = soTarget.FindProperty("ignoreChildUnSelectModuleContainers");

            onSelectModuleContainers = soTarget.FindProperty("selectedModuleContainers");
            applySelectedModuleContainers = soTarget.FindProperty("applySelectedModuleContainers");
            ignoreChildSelectedModuleContainers = soTarget.FindProperty("ignoreChildSelectedModuleContainers");

            beingPressedModuleContainers = soTarget.FindProperty("beingPressedModuleContainers");
            applyBeingPressedModuleContainers = soTarget.FindProperty("applyBeingPressedModuleContainers");
            ignoreChildBeingPressedModuleContainers = soTarget.FindProperty("ignoreChildBeingPressedModuleContainers");

            pressCompleteModuleContainers = soTarget.FindProperty("pressCompleteModuleContainers");
            applyPressCompleteModuleContainers = soTarget.FindProperty("applyPressCompleteModuleContainers");
            ignoreChildPressCompleteModuleContainers = soTarget.FindProperty("ignoreChildPressCompleteModuleContainers");
        }

        void GetStyleReferences()
        {
            UseStyle = soTarget.FindProperty("_useStyle");

            useNormalItemVisual = soTarget.FindProperty("_useNormalItemVisual");
            normalTextSize = soTarget.FindProperty("_normalTextSize");
            normalItemTextMaterial = soTarget.FindProperty("_normalTextMaterial");
            normalItemBackgroundMaterial = soTarget.FindProperty("_normalBackgroundMaterial");

            useSelectedItemVisual = soTarget.FindProperty("_useSelectedItemVisual");
            selectedItemFontSize = soTarget.FindProperty("_selectedTextSize");
            selectedItemTextMaterial = soTarget.FindProperty("_selectedTextMaterial");
            selectedItemBackgroundMaterial = soTarget.FindProperty("_selectedBackgroundMaterial");

            usePressedItemVisual = soTarget.FindProperty("_usePressedItemVisual");
            pressedItemFontSize = soTarget.FindProperty("_pressedTextSize");
            pressedItemTextMaterial = soTarget.FindProperty("_pressedTextMaterial");
            pressedItemBackgroundMaterial = soTarget.FindProperty("_pressedBackgroundMaterial");
            holdPressedVisualFor = soTarget.FindProperty("holdPressedVisualFor");

            useDisabledItemVisual = soTarget.FindProperty("_useDisabledItemVisual");
            disabledItemFontSize = soTarget.FindProperty("_disabledTextSize");
            disabledItemTextMaterial = soTarget.FindProperty("_disabledTextMaterial");
            disabledItemBackgroundMaterial = soTarget.FindProperty("_disabledBackgroundMaterial");
        }

        void GetAnimBoolReferences()
        {
            showLayoutSettingsInEditor = new AnimBool(false);
            showLayoutSettingsInEditor.valueChanged.AddListener(Repaint);
            showModuleSettingsInEditor = new AnimBool(false);
            showModuleSettingsInEditor.valueChanged.AddListener(Repaint);
            showDebugSettingsInEditor = new AnimBool(false);
            showDebugSettingsInEditor.valueChanged.AddListener(Repaint);
            showDisabledItemSettings = new AnimBool(false);
            showDisabledItemSettings.valueChanged.AddListener(Repaint);
            showPressedItemSettings = new AnimBool(false);
            showPressedItemSettings.valueChanged.AddListener(Repaint);
            showSelectedItemSettings = new AnimBool(false);
            showSelectedItemSettings.valueChanged.AddListener(Repaint);
            showNormalItemSettings = new AnimBool(false);
            showNormalItemSettings.valueChanged.AddListener(Repaint);
            showChildVisualSettings = new AnimBool(false);
            showChildVisualSettings.valueChanged.AddListener(Repaint);

            showInputSettings = new AnimBool(false);
            showInputSettings.valueChanged.AddListener(Repaint);
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
            if (defaultLabelStyle == null)
            {
                defaultLabelStyle = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 11
                };
                defaultLabelStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
            }
            headerLabel ??= new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            iconButtonStyle ??= new GUIStyle(EditorStyles.toolbarButton);
        }
    }
}
