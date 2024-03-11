using System;
using TinyGiantStudio.EditorHelpers;
using TinyGiantStudio.Modules;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(Button))]
    public class ButtonEditor : Editor
    {
        public AssetSettings settings;

        Button myTarget;
        SerializedObject soTarget;

        SerializedProperty pressCompleteEvent;
        SerializedProperty beingPressedEvent;
        SerializedProperty selectedEvent;
        SerializedProperty unselectEvent;

        SerializedProperty interactable;
        SerializedProperty interactableByMouse;

        SerializedProperty text;
        SerializedProperty background;

        SerializedProperty useStyles;

        SerializedProperty normalTextSize;
        SerializedProperty normalTextMaterial;
        SerializedProperty normalBackgroundMaterial;

        SerializedProperty useSelectedVisual;
        SerializedProperty selectedTextSize;
        SerializedProperty selectedTextMaterial;
        SerializedProperty selectedBackgroundMaterial;

        SerializedProperty usePressedVisual;
        SerializedProperty pressedTextSize;
        SerializedProperty pressedTextMaterial;
        SerializedProperty pressedBackgroundMaterial;
        SerializedProperty holdPressedVisualFor;


        SerializedProperty useDisabledVisual;
        SerializedProperty disabledTextSize;
        SerializedProperty disabledTextMaterial;
        SerializedProperty disabledBackgroundMaterial;

        SerializedProperty useModules;

        SerializedProperty unSelectedModuleContainers;
        SerializedProperty applyUnSelectedModuleContainers;
        SerializedProperty selectedModuleContainers;
        SerializedProperty applySelectedModuleContainers;
        SerializedProperty beingPressedModuleContainers;
        SerializedProperty applyBeingPressedModuleContainers;
        SerializedProperty pressCompleteModuleContainers;
        SerializedProperty applyPressCompleteModuleContainers;

        SerializedProperty hideOverwrittenVariablesFromInspector;


        GUIStyle foldOutStyle = null;
        GUIStyle iconButtonStyle = null;
        GUIStyle defaultLabel = null;
        GUIStyle defaultMultilineLabel = null;
        GUIStyle headerLabel = null;
        GUIStyle myStyleButton = null;


        static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);
        static Color toggledOffColor = new Color(0.75f, 0.75f, 0.75f); //settings that are turned off but still visible

        AnimBool showEventSettings;
        AnimBool showModuleSettings;
        AnimBool showAdvancedSettings;

        AnimBool showDisabledItemSettings;
        AnimBool showPressedItemSettings;
        AnimBool showSelectedItemSettings;
        AnimBool showNormalItemSettings;
        AnimBool showVisualSettings;

        Texture documentationIcon;
        readonly float iconSize = 20;

        readonly string variableName = "\n\nVariable name: ";

        Type xrToolkitSetupClass;


        void OnEnable()
        {
            xrToolkitSetupClass = Type.GetType("TinyGiantStudio.Text.XRToolkitEditorSetup");

            GetReferences();

            documentationIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Documentation.png") as Texture;

            if (!settings)
                settings = StaticMethods.VerifySettings(settings);
        }

        public override void OnInspectorGUI()
        {
            GenerateStyle();
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            Warning();
            GUILayout.Space(10);
            MainSettings();
            GUILayout.Space(10);
            Events();
            GUILayout.Space(6);
            Styles();
            GUILayout.Space(6);
            ModuleSettings();
            GUILayout.Space(6);
            AdvancedSettings();

            if (xrToolkitSetupClass != null)
                xrToolkitSetupClass.GetMethod("CreateSetupButton").Invoke(null, new object[] { myTarget.gameObject, myStyleButton });

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();

                myTarget.UpdateStyle();

                EditorUtility.SetDirty(myTarget);
            }
        }
        void Warning()
        {
            if (myTarget.ApplyNormalStyle().Item1 || myTarget.ApplyOnSelectStyle().Item1 || myTarget.ApplyPressedStyle().Item1 || myTarget.ApplyDisabledStyle().Item1)
                EditorGUILayout.HelpBox("Some values are overwritten by parent list.", MessageType.Info);
        }


        void MainSettings()
        {
            MText_Editor_Methods.ItalicHorizontalField(text, "Text", "Style changes apply to the text specified here." + variableName + "Text");
            MText_Editor_Methods.ItalicHorizontalField(background, "Background", "The background is assigned to the button. The button can change material depending on Style." + variableName + "Background");
            //EditorGUILayout.PropertyField(background);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            float defaultLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.PropertyField(interactable);
            if (myTarget.interactable)
            {
                EditorGUIUtility.labelWidth = 120;
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(interactableByMouse, new GUIContent("By mouse/touch"));
            }
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            GUILayout.EndHorizontal();
        }

        void Styles()
        {
            if (myTarget.hideOverwrittenVariablesFromInspector && myTarget.ApplyNormalStyle().Item1 && myTarget.ApplyOnSelectStyle().Item1 && myTarget.ApplyPressedStyle().Item1 && myTarget.ApplyDisabledStyle().Item1)
                return;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useStyles, GUIContent.none, GUILayout.MaxWidth(25));
            showVisualSettings.target = EditorGUILayout.Foldout(showVisualSettings.target, new GUIContent("Style", "If set to false, disables all style controls from this button. \n\nThe name of the bool: 'useStyle'"), true, foldOutStyle);
            Documentation("https://ferdowsur.gitbook.io/modular-3d-text/utility/ui-states", "Understanding UI States");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showVisualSettings.faded))
            {
                GUILayout.Space(8);
                NormalStyle();
                GUILayout.Space(5);
                SelectedStyle();
                GUILayout.Space(5);
                PressedItemSettings();
                GUILayout.Space(5);
                DisabledtStyle();
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        void Events()
        {
            EditorGUI.indentLevel = 2;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Events",
                           "Unselected :" +
                           "\nVariable name: unselectEvent" +
                           "\nThis is the state that appears when a UI element enters the normal state from selected." +
                           "\n" +
                           "\nSelected :" +
                           "\nVariable name: selectedEvent" +
                           "\nMouse hover on the item or selected item via keyboard/controller in a list." +
                           "\n" +
                           "\nBeing Pressed :" +
                           "\nVariable name: beingPressedEvent" +
                           "\nWhile the mouse click or touch is held down, the module or event is constantly called." +
                           "\n" +
                           "\nPress Complete:" +
                           "\nVariable name: pressCompleteEvent" +
                           "\nWhen the user releases the key. In other words, the frame when the button/key or touch being pressed is complete." +
                           "");

            showEventSettings.target = EditorGUILayout.Foldout(showEventSettings.target, content, true, foldOutStyle);
            Documentation("https://ferdowsur.gitbook.io/modular-3d-text/utility/ui-states", "Understanding UI States");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showEventSettings.faded))
            {
                EditorGUILayout.PropertyField(unselectEvent, new GUIContent("Unselected", "This is the state that appears when a UI element enters the normal state from selected. \r\nThis is used instead of normal because a UI element can be \"normal\" state by default when the game starts."));
                EditorGUILayout.PropertyField(selectedEvent, new GUIContent("Selected", "Mouse hover on the item or selected item via keyboard/controller in a list."));
                EditorGUILayout.PropertyField(beingPressedEvent, new GUIContent("Being Pressed", "While the mouse/keyboard/controller button is pressed down or touch held down, the module or event is constantly called."));
                EditorGUILayout.PropertyField(pressCompleteEvent, new GUIContent("Press Complete", "When the user releases the key. In other words, the frame when the button/key or touch being pressed is complete."));
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
            showModuleSettings.target = EditorGUILayout.Foldout(showModuleSettings.target, new GUIContent("Modules", "If set to false, disables all modules from this button. Modules are called when entering a style for the first time." + variableName + "useModules"), true, foldOutStyle);

            Documentation("https://ferdowsur.gitbook.io/modular-3d-text/modules", "Modules");

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            string tooltip_unSelected = "Unselect is the state that appears when a UI element enters the normal state from selected.";
            string tooltip_selected = "Selected is the state when Mouse hovers on a UI item or it is selected via keyboard/controller by scrolling in a list.";
            string tooltip_beingPressed = "Being pressed is while the mouse click or touch is held down, the module or event is constantly called.";
            string tooltip_onClick = "When the user releases the key.";

            Color contentDefaultColor = GUI.contentColor;
            if (!myTarget.useModules)
                GUI.contentColor = toggledOffColor;

            if (EditorGUILayout.BeginFadeGroup(showModuleSettings.faded))
            {
                GUILayout.Space(5);
                EditorGUI.indentLevel = 2;
                ModuleDrawer.BaseModuleContainerList("UnSelected", tooltip_unSelected + variableName + "applyUnSelectedModuleContainers for enabling/disabling." + variableName + "unSelectedModuleContainers for ModuleContainer List", myTarget.unSelectedModuleContainers, unSelectedModuleContainers, soTarget, applyUnSelectedModuleContainers);
                GUILayout.Space(10);
                ModuleDrawer.BaseModuleContainerList("Selected", tooltip_selected + variableName + "applySelectModuleContainers for enabling/disabling." + variableName + "selectedModuleContainers for ModuleContainer List", myTarget.selectedModuleContainers, selectedModuleContainers, soTarget, applySelectedModuleContainers);
                GUILayout.Space(10);
                ModuleDrawer.BaseModuleContainerList("Being Pressed", tooltip_beingPressed + variableName + "applyBeingPressedModuleContainers for enabling/disabling." + variableName + "beingPressedModuleContainers for ModuleContainer List", myTarget.beingPressedModuleContainers, beingPressedModuleContainers, soTarget, applyBeingPressedModuleContainers);
                GUILayout.Space(10);
                ModuleDrawer.BaseModuleContainerList("Press Complete", tooltip_onClick + variableName + "applyPressCompleteModuleContainers for enabling/disabling." + variableName + "pressCompleteModuleContainers for ModuleContainer List", myTarget.pressCompleteModuleContainers, pressCompleteModuleContainers, soTarget, applyPressCompleteModuleContainers);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();

            GUI.contentColor = contentDefaultColor;
        }

        void AdvancedSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            showAdvancedSettings.target = EditorGUILayout.Foldout(showAdvancedSettings.target, new GUIContent("Advanced settings", ""), true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showAdvancedSettings.faded))
            {
                EditorGUI.indentLevel = 1;
                MText_Editor_Methods.HorizontalField(hideOverwrittenVariablesFromInspector, "Hide overwritten values", "Buttons under list sometimes have styles overwritten. This hides these variables", FieldSize.extraLarge);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }


        void NormalStyle()
        {
            if (myTarget.ApplyNormalStyle().Item1 && myTarget.hideOverwrittenVariablesFromInspector)
                return;

            Color contentDefaultColor = GUI.contentColor;
            if (!myTarget.useStyles)
                GUI.contentColor = toggledOffColor;

            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            showNormalItemSettings.target = EditorGUILayout.Foldout(showNormalItemSettings.target, new GUIContent("Normal"), true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showNormalItemSettings.faded))
            {
                EditorGUI.indentLevel = 0;

                if (myTarget.ApplyNormalStyle().Item1)
                {
                    EditorGUILayout.HelpBox("Normal style visuals are being overwritten by parent list", MessageType.Info);
                    GUILayout.Space(5);
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Text Size", variableName + "NormalTextSize"), defaultLabel, GUILayout.MaxWidth(70));
                EditorGUILayout.PropertyField(normalTextSize, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.PreviewField(normalTextMaterial, myTarget.NormalTextMaterial, "Text", variableName + "NormalTextMaterial");
                MText_Editor_Methods.PreviewField(normalBackgroundMaterial, myTarget.NormalBackgroundMaterial, "Background", variableName + "NormalBackgroundMaterial");
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();

            GUI.contentColor = contentDefaultColor;
        }
        void SelectedStyle()
        {
            if (myTarget.ApplyOnSelectStyle().Item1 && myTarget.hideOverwrittenVariablesFromInspector)
                return;

            Color contentDefaultColor = GUI.contentColor;
            if (!myTarget.useStyles || !myTarget.useSelectedVisual)
                GUI.contentColor = toggledOffColor;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Selected", "Mouse hover or selected in a list ready to be clicked" + variableName + "useSelectedVisual");
            EditorGUILayout.PropertyField(useSelectedVisual, GUIContent.none, GUILayout.MaxWidth(25));
            showSelectedItemSettings.target = EditorGUILayout.Foldout(showSelectedItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showSelectedItemSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                if (myTarget.ApplyOnSelectStyle().Item1)
                {
                    EditorGUILayout.HelpBox("On select style visuals are being overwritten by parent list", MessageType.Info);
                    GUILayout.Space(5);
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Text Size", variableName + "SelectedTextSize"), defaultLabel, GUILayout.MaxWidth(70));
                EditorGUILayout.PropertyField(selectedTextSize, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.PreviewField(selectedTextMaterial, myTarget.SelectedTextMaterial, "Text", variableName + "SelectedTextMaterial");
                MText_Editor_Methods.PreviewField(selectedBackgroundMaterial, myTarget.SelectedBackgroundMaterial, "Background", variableName + "SelectedBackgroundMaterial");
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
            GUI.contentColor = contentDefaultColor;
        }
        void PressedItemSettings()
        {
            if (myTarget.ApplyPressedStyle().Item1 && myTarget.hideOverwrittenVariablesFromInspector)
                return;

            Color contentDefaultColor = GUI.contentColor;
            if (!myTarget.useStyles || !myTarget.usePressedVisual)
                GUI.contentColor = toggledOffColor;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Pressed", "When click/tocuh is pressed down or for limited time after click." + variableName + "usePressedVisual");
            EditorGUILayout.PropertyField(usePressedVisual, GUIContent.none, GUILayout.MaxWidth(25));
            showPressedItemSettings.target = EditorGUILayout.Foldout(showPressedItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showPressedItemSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                if (myTarget.ApplyPressedStyle().Item1)
                {
                    EditorGUILayout.HelpBox("Pressed style visuals are being overwritten by parent list", MessageType.Info);
                    GUILayout.Space(5);
                }
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Text Size", variableName + "PressedTextSize"), defaultLabel, GUILayout.MaxWidth(70));
                EditorGUILayout.PropertyField(pressedTextSize, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.PreviewField(pressedTextMaterial, myTarget.PressedTextMaterial, "Text", variableName + "PressedTextMaterial");
                MText_Editor_Methods.PreviewField(pressedBackgroundMaterial, myTarget.PressedBackgroundMaterial, "Background", variableName + "PressedBackgroundMaterial");
                GUILayout.EndHorizontal();

                MText_Editor_Methods.HorizontalField(holdPressedVisualFor, "Hold pressed for", "How long this visual lasts after pressing. This is not for mouse/touch click." + variableName + "holdPressedVisualFor", FieldSize.large);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
            GUI.contentColor = contentDefaultColor;
        }
        void DisabledtStyle()
        {
            if (myTarget.ApplyDisabledStyle().Item1 && myTarget.hideOverwrittenVariablesFromInspector)
                return;

            Color contentDefaultColor = GUI.contentColor;
            if (!myTarget.useStyles || !myTarget.UseDisabledVisual)
                GUI.contentColor = toggledOffColor;


            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Disabled", "Style when button isn't interactable." + variableName + "UseDisabledVisual");
            EditorGUILayout.PropertyField(useDisabledVisual, GUIContent.none, GUILayout.MaxWidth(25));
            showDisabledItemSettings.target = EditorGUILayout.Foldout(showDisabledItemSettings.target, content, true, foldOutStyle);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDisabledItemSettings.faded))
            {
                EditorGUI.indentLevel = 0;
                if (myTarget.ApplyDisabledStyle().Item1)
                {
                    EditorGUILayout.HelpBox("Disabled style visuals are being overwritten by parent list", MessageType.Info);
                    GUILayout.Space(5);
                }
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Text Size", variableName + "DisabledTextSize"), defaultLabel, GUILayout.MaxWidth(70));
                EditorGUILayout.PropertyField(disabledTextSize, GUIContent.none);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                MText_Editor_Methods.PreviewField(disabledTextMaterial, myTarget.DisabledTextMaterial, "Text", variableName + "DisabledTextMaterial");
                MText_Editor_Methods.PreviewField(disabledBackgroundMaterial, myTarget.DisabledBackgroundMaterial, "Background", variableName + "DisabledBackgroundMaterial");
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
            GUI.contentColor = contentDefaultColor;
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
            myTarget = (Button)target;
            soTarget = new SerializedObject(target);

            showModuleSettings = new AnimBool(false);
            showModuleSettings.valueChanged.AddListener(Repaint);
            showAdvancedSettings = new AnimBool(false);
            showAdvancedSettings.valueChanged.AddListener(Repaint);


            unselectEvent = soTarget.FindProperty("unselectEvent");
            selectedEvent = soTarget.FindProperty("selectedEvent");
            beingPressedEvent = soTarget.FindProperty("beingPressedEvent");
            pressCompleteEvent = soTarget.FindProperty("pressCompleteEvent");

            interactable = soTarget.FindProperty("interactable");
            interactableByMouse = soTarget.FindProperty("interactableByMouse");

            text = soTarget.FindProperty("_text");
            background = soTarget.FindProperty("_background");


            useStyles = soTarget.FindProperty("useStyles");

            normalTextSize = soTarget.FindProperty("_normalTextSize");
            normalTextMaterial = soTarget.FindProperty("_normalTextMaterial");
            normalBackgroundMaterial = soTarget.FindProperty("_normalBackgroundMaterial");

            useSelectedVisual = soTarget.FindProperty("useSelectedVisual");
            selectedTextSize = soTarget.FindProperty("_selectedTextSize");
            selectedTextMaterial = soTarget.FindProperty("_selectedTextMaterial");
            selectedBackgroundMaterial = soTarget.FindProperty("_selectedBackgroundMaterial");

            usePressedVisual = soTarget.FindProperty("usePressedVisual");
            pressedTextSize = soTarget.FindProperty("_pressedTextSize");
            pressedTextMaterial = soTarget.FindProperty("_pressedTextMaterial");
            pressedBackgroundMaterial = soTarget.FindProperty("_pressedBackgroundMaterial");
            holdPressedVisualFor = soTarget.FindProperty("holdPressedVisualFor");

            useDisabledVisual = soTarget.FindProperty("_useDisabledVisual");
            disabledTextSize = soTarget.FindProperty("_disabledTextSize");
            disabledTextMaterial = soTarget.FindProperty("_disabledTextMaterial");
            disabledBackgroundMaterial = soTarget.FindProperty("_disabledBackgroundMaterial");



            useModules = soTarget.FindProperty("useModules");

            unSelectedModuleContainers = soTarget.FindProperty("unSelectedModuleContainers");
            applyUnSelectedModuleContainers = soTarget.FindProperty("applyUnSelectedModuleContainers");
            selectedModuleContainers = soTarget.FindProperty("selectedModuleContainers");
            applySelectedModuleContainers = soTarget.FindProperty("applySelectModuleContainers");
            beingPressedModuleContainers = soTarget.FindProperty("beingPressedModuleContainers");
            applyBeingPressedModuleContainers = soTarget.FindProperty("applyBeingPressedModuleContainers");
            pressCompleteModuleContainers = soTarget.FindProperty("pressCompleteModuleContainers");
            applyPressCompleteModuleContainers = soTarget.FindProperty("applyPressCompleteModuleContainers");

            hideOverwrittenVariablesFromInspector = soTarget.FindProperty("hideOverwrittenVariablesFromInspector");

            GetAnimBoolReferences();
        }
        void GetAnimBoolReferences()
        {
            showEventSettings = new AnimBool(false);
            showEventSettings.valueChanged.AddListener(Repaint);

            showVisualSettings = new AnimBool(false);
            showVisualSettings.valueChanged.AddListener(Repaint);

            showNormalItemSettings = new AnimBool(false);
            showNormalItemSettings.valueChanged.AddListener(Repaint);

            showSelectedItemSettings = new AnimBool(false);
            showSelectedItemSettings.valueChanged.AddListener(Repaint);

            showPressedItemSettings = new AnimBool(false);
            showPressedItemSettings.valueChanged.AddListener(Repaint);

            showDisabledItemSettings = new AnimBool(false);
            showDisabledItemSettings.valueChanged.AddListener(Repaint);
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
                    fontStyle = FontStyle.Bold,
                };
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }

            if (defaultLabel == null)
            {
                defaultLabel = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 11
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
                //defaultMultilineLabel.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
            }
            if (headerLabel == null)
            {
                headerLabel = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                };
            }

            iconButtonStyle ??= new GUIStyle(EditorStyles.toolbarButton);

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

        //void DrawUILine(Color color, int thickness = 1, int padding = 0)
        //{
        //    Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        //    r.height = thickness;
        //    r.y += padding / 2;
        //    r.x -= 2;
        //    r.width += 6;
        //    EditorGUI.DrawRect(r, color);
        //}
    }
}