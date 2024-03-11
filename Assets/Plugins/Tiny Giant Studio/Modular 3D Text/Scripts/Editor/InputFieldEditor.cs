using System;
using TinyGiantStudio.EditorHelpers;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(InputField))]
    public class InputFieldEditor : Editor
    {
        public AssetSettings settings;

        readonly float defaultSmallHorizontalFieldSize = 72.5f;
        readonly float defaultNormalltHorizontalFieldSize = 100;
        readonly float defaultLargeHorizontalFieldSize = 120f;
        readonly float defaultExtraLargeHorizontalFieldSize = 150f;


        InputField myTarget;
        SerializedObject soTarget;

        SerializedProperty autoFocusOnGameStart;
        SerializedProperty interactable;

        SerializedProperty maxCharacter;
        SerializedProperty caret;
        SerializedProperty hideCaretIfMaxCharacter;

        SerializedProperty enterKeyEndsInput;

        SerializedProperty contentType;

        SerializedProperty textComponent;
        SerializedProperty background;

        SerializedProperty text;
        SerializedProperty placeHolderText;

        SerializedProperty placeHolderTextMat;

        SerializedProperty inFocusTextMat;
        SerializedProperty inFocusBackgroundMat;

        SerializedProperty outOfFocusTextMat;
        SerializedProperty outOfFocusBackgroundMat;

        SerializedProperty hoveredBackgroundMat;

        SerializedProperty disabledTextMat;
        SerializedProperty disabledBackgroundMat;

        SerializedProperty typeSound;
        SerializedProperty audioSource;

        SerializedProperty onInput;
        SerializedProperty onBackspace;
        SerializedProperty onInputEnd;


        AnimBool showMainSettings;
        AnimBool showStyleSettings;
        AnimBool showAudioSettings;
        AnimBool showUnityEventSettings;


        GUIStyle foldOutStyle;
        GUIStyle defaultLabel = null;
        GUIStyle defaultMultilineLabel = null;
        GUIStyle headerLabel = null;
        GUIStyle iconButtonStyle = null;
        GUIStyle myStyleButton = null;

        readonly float iconSize = 20;
        Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 1);
        Texture documentationIcon;

        readonly string variableName = "\n\nVariable name: ";

        Type xrToolkitSetupClass;


        void OnEnable()
        {
            myTarget = (InputField)target;
            soTarget = new SerializedObject(target);

            FindProperties();

            documentationIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Documentation.png") as Texture;
            xrToolkitSetupClass = Type.GetType("TinyGiantStudio.Text.XRToolkitEditorSetup");

            AnimBools();
        }



        public override void OnInspectorGUI()
        {
            GenerateStyle();
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            MainSettings();
            EditorGUILayout.Space(5);
            StyleSettings();
            EditorGUILayout.Space(5);
            AudioSettings();
            EditorGUILayout.Space(5);
            UnityEventsSettings();

            if (xrToolkitSetupClass != null)
                xrToolkitSetupClass.GetMethod("CreateSetupButton").Invoke(null, new object[] { myTarget.gameObject, myStyleButton });

            //if (Application.isPlaying)
            //{
            //    GUILayout.Label(myTarget.state.State.ToString());
            //    GUILayout.Space(5);
            //}


            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                ApplyModifiedValuesToGraphics();
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
                EditorGUI.indentLevel = 0;

                MText_Editor_Methods.ItalicHorizontalField(text, "Text", "The current text in the inputfield.\r\nDirectly assigning the Text property makes a sound If it has both Audioclip and Audio Source." + variableName + "Text");
                MText_Editor_Methods.ItalicHorizontalField(placeHolderText, "Placeholder", "The text to show when the actual text to show is empty.\r\nFor example, if an input field for a username is empty, the placeholder can show \"User name\" to indicate to the user that this is a field for a username." + variableName + "placeHolderText");
                EditorGUILayout.Space(1);
                MText_Editor_Methods.ItalicHorizontalField(enterKeyEndsInput, "'Enter' ends Input", variableName + "enterKeyEndsInput", FieldSize.large);
                MText_Editor_Methods.ItalicHorizontalField(caret, "Caret", variableName + "caret");
                MText_Editor_Methods.ItalicHorizontalField(maxCharacter, "Max Char", "The maximum amount of character allowed in the input field." + variableName + "maxCharacter");
                MText_Editor_Methods.ItalicHorizontalField(hideCaretIfMaxCharacter, "Hide caret if max char", "This hides the typing symbol when max character has been typed." + variableName + hideCaretIfMaxCharacter, FieldSize.gigantic);

                EditorGUILayout.Space(5);
                MText_Editor_Methods.ItalicHorizontalField(contentType, "Content Type", variableName + "contentType");
                EditorGUILayout.Space(5);

                if (!StaticMethods.GetParentList(myTarget.transform))
                {
                    MText_Editor_Methods.ItalicHorizontalField(autoFocusOnGameStart, "Auto Focus", "If set to true, this is focused on awake. The slider is scrollable with a keyboard when focused.\r\nIf it is in a list, the list controls who to focus on." + variableName + "autoFocusOnGameStart", FieldSize.small);
                }
                MText_Editor_Methods.ItalicHorizontalField(interactable, "Interactable", variableName + "interactable", FieldSize.small);
                EditorGUILayout.Space(10);

                if (!myTarget.textComponent)
                    EditorGUILayout.HelpBox("Text Component is required", MessageType.Error);
                HorizontalField(textComponent, "Text Component", "Reference to the 3D Text where input will be shown." + variableName + "textComponent");
                HorizontalField(background, "Background", variableName + "background");
                EditorGUILayout.Space(10);
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }
        void StyleSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            GUIContent content = new GUIContent("Style");
            showStyleSettings.target = EditorGUILayout.Foldout(showStyleSettings.target, content, true, foldOutStyle);
            Documentation("https://ferdowsur.gitbook.io/modular-3d-text/utility/ui-states", "Understanding UI States");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showStyleSettings.faded))
            {
                GUILayout.Space(5);
                TextMats();
                GUILayout.Space(5);
                BackgroundMats();
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        void TextMats()
        {
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Text Metarial", headerLabel);
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            GUILayout.Space(5);


            GUILayout.BeginHorizontal();
            MText_Editor_Methods.PreviewField(placeHolderTextMat, myTarget.placeHolderTextMat, "Placeholder Text", variableName + "placeHolderTextMat");
            MText_Editor_Methods.PreviewField(disabledTextMat, myTarget.disabledTextMat, "Disabled Text", variableName + "disabledTextMat");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            MText_Editor_Methods.PreviewField(inFocusTextMat, myTarget.inFocusTextMat, "In Focus Text", variableName + "inFocusTextMat");
            MText_Editor_Methods.PreviewField(outOfFocusTextMat, myTarget.outOfFocusTextMat, "Normal Text", variableName + "outOfFocusTextMat");
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
        }

        void BackgroundMats()
        {
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Background Metarial", headerLabel);
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            GUILayout.Space(5);


            GUILayout.BeginHorizontal();
            MText_Editor_Methods.PreviewField(inFocusBackgroundMat, myTarget.inFocusBackgroundMat, "In Focus", variableName + "inFocusBackgroundMat");
            MText_Editor_Methods.PreviewField(outOfFocusBackgroundMat, myTarget.normalBackgroundMaterial, "Normal Focus", variableName + "outOfFocusBackgroundMat");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            MText_Editor_Methods.PreviewField(hoveredBackgroundMat, myTarget.hoveredBackgroundMaterial, "Hovered", variableName + "hoveredBackgroundMat");
            MText_Editor_Methods.PreviewField(disabledBackgroundMat, myTarget.disabledBackgroundMat, "Disabled", variableName + "disabledBackgroundMat");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void AudioSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUIContent content = new GUIContent("Audio");
            showAudioSettings.target = EditorGUILayout.Foldout(showAudioSettings.target, content, true, foldOutStyle);
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showAudioSettings.faded))
            {
                EditorGUI.indentLevel = 0;

                HorizontalField(typeSound, "Type Sound");
                HorizontalField(audioSource, "Audio Source");
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        void UnityEventsSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUIContent content = new GUIContent("Events");
            showUnityEventSettings.target = EditorGUILayout.Foldout(showUnityEventSettings.target, content, true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showUnityEventSettings.faded))
            {
                EditorGUILayout.PropertyField(onInput);
                EditorGUILayout.PropertyField(onBackspace);
                EditorGUILayout.PropertyField(onInputEnd);
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        void ApplyModifiedValuesToGraphics()
        {
            if (!myTarget.interactable)
                myTarget.UninteractableUsedByEditorOnly();
            else
                myTarget.InteractableUsedByEditorOnly();
        }





        void FindProperties()
        {
            autoFocusOnGameStart = soTarget.FindProperty("autoFocusOnGameStart");
            interactable = soTarget.FindProperty("interactable");

            maxCharacter = soTarget.FindProperty("maxCharacter");

            caret = soTarget.FindProperty("caret");
            hideCaretIfMaxCharacter = soTarget.FindProperty("hideCaretIfMaxCharacter");

            enterKeyEndsInput = soTarget.FindProperty("enterKeyEndsInput");
            contentType = soTarget.FindProperty("contentType");

            textComponent = soTarget.FindProperty("textComponent");
            background = soTarget.FindProperty("background");

            text = soTarget.FindProperty("_text");
            placeHolderText = soTarget.FindProperty("placeHolderText");


            placeHolderTextMat = soTarget.FindProperty("placeHolderTextMat");

            inFocusTextMat = soTarget.FindProperty("inFocusTextMat");
            inFocusBackgroundMat = soTarget.FindProperty("inFocusBackgroundMat");

            outOfFocusTextMat = soTarget.FindProperty("outOfFocusTextMat");
            outOfFocusBackgroundMat = soTarget.FindProperty("normalBackgroundMaterial");

            hoveredBackgroundMat = soTarget.FindProperty("hoveredBackgroundMaterial");

            disabledTextMat = soTarget.FindProperty("disabledTextMat");
            disabledBackgroundMat = soTarget.FindProperty("disabledBackgroundMat");


            typeSound = soTarget.FindProperty("typeSound");
            audioSource = soTarget.FindProperty("audioSource");

            onInput = soTarget.FindProperty("onInput");
            onBackspace = soTarget.FindProperty("onBackspace");
            onInputEnd = soTarget.FindProperty("onInputEnd");
        }
        void AnimBools()
        {
            showMainSettings = new AnimBool(true);
            showMainSettings.valueChanged.AddListener(Repaint);

            showStyleSettings = new AnimBool(false);
            showStyleSettings.valueChanged.AddListener(Repaint);

            showAudioSettings = new AnimBool(false);
            showAudioSettings.valueChanged.AddListener(Repaint);

            showUnityEventSettings = new AnimBool(false);
            showUnityEventSettings.valueChanged.AddListener(Repaint);
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


            if (defaultLabel == null)
            {
                defaultLabel = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 12
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
            if (headerLabel == null)
            {
                headerLabel = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                };
            }
            iconButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);

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

        void HorizontalField(SerializedProperty property, string label, string tooltip = "", FieldSize fieldSize = FieldSize.normal)
        {
            float myMaxWidth;
            //not to self: it's ternary operator not tarnary operator. Stop mistyping
            if (settings)
                myMaxWidth = fieldSize == FieldSize.small ? settings.smallHorizontalFieldSize : fieldSize == FieldSize.normal ? settings.normalHorizontalFieldSize : fieldSize == FieldSize.large ? settings.largeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? settings.extraLargeHorizontalFieldSize : settings.normalHorizontalFieldSize;
            else
                myMaxWidth = fieldSize == FieldSize.small ? defaultSmallHorizontalFieldSize : fieldSize == FieldSize.normal ? defaultNormalltHorizontalFieldSize : fieldSize == FieldSize.large ? defaultLargeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? defaultExtraLargeHorizontalFieldSize : settings.normalHorizontalFieldSize;

            GUILayout.BeginHorizontal();
            GUIContent gUIContent = new GUIContent(label, tooltip);
            EditorGUILayout.LabelField(gUIContent, GUILayout.MaxWidth(myMaxWidth));
            EditorGUILayout.PropertyField(property, GUIContent.none);
            GUILayout.EndHorizontal();
        }

        void Documentation(string URL, string subject)
        {
            GUIContent doc = new GUIContent(documentationIcon, subject + " documentation\n\nURL: " + URL);
            if (GUILayout.Button(doc, iconButtonStyle, GUILayout.Height(iconSize), GUILayout.Width(iconSize)))
            {
                Application.OpenURL(URL);
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