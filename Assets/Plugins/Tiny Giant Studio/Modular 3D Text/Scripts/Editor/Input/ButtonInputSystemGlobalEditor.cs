using TinyGiantStudio.EditorHelpers;
using UnityEditor;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(ButtonInputSystemGlobal))]
    public class ButtonInputSystemGlobalEditor : Editor
    {
        ButtonInputSystemGlobal myTarget;
        SerializedObject soTarget;

        SerializedProperty buttonInputProcessorStyle;
        SerializedProperty selectedInputSystemController;
        SerializedProperty debugInputs;
#if ENABLE_INPUT_SYSTEM
        SerializedProperty inputActionAsset;
#else
        SerializedProperty tickRate;
#endif



        void OnEnable()
        {
            myTarget = (ButtonInputSystemGlobal)target;
            soTarget = new SerializedObject(target);

            GetReferences();
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            //GenerateStyle();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(buttonInputProcessorStyle, GUIContent.none);
            Notice();
            GUILayout.Space(5);
#if ENABLE_INPUT_SYSTEM
            MText_Editor_Methods.ItalicHorizontalField(inputActionAsset, "Input Action Asset", "If it is null, in editor, it is updated from settings file.", FieldSize.large);
#else
            if (myTarget.MyButtonInputProcessorStyle == ButtonInputSystemGlobal.ButtonInputProcessorStyle.CommonInputController)
                MText_Editor_Methods.ItalicHorizontalField(tickRate, "Tick Rate", "How long you have to press a key down for it to register as a second key press", FieldSize.normal);
#endif
            if (Application.isPlaying)
                MText_Editor_Methods.ItalicHorizontalField(selectedInputSystemController, "Currently Selected", "This is assigned when InputSystemController is enabled/disabled and when a list with autofocus is instantiated on void start()", FieldSize.large);
            MText_Editor_Methods.ItalicHorizontalField(debugInputs, "Debug inputs", "Will spamm debug logs.", FieldSize.large);

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                myTarget.UpdateButtonInputProcessorScript(false);
                EditorApplication.delayCall += () => myTarget.SetupInputProcessor();

                EditorUtility.SetDirty(myTarget);
            }
        }

        void Notice()
        {
            if (myTarget.MyButtonInputProcessorStyle == ButtonInputSystemGlobal.ButtonInputProcessorStyle.IndividualPlayerInputComponents)
            {
#if ENABLE_INPUT_SYSTEM
                EditorGUILayout.HelpBox("Please make sure all the list has Player Input component for them to work. Just click the 'Update input system' button in the inspector on a list.", MessageType.Info);

                GUILayout.Space(5);
#endif
            }
            else if (myTarget.MyButtonInputProcessorStyle == ButtonInputSystemGlobal.ButtonInputProcessorStyle.CommonInputController)
            {
#if ENABLE_INPUT_SYSTEM

#if UNITY_2023_1_OR_NEWER
                PlayerInput[] playerInputs = UnityEngine.Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
#else
                PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
#endif
                if (playerInputs.Length > 0)
                {
                    EditorGUILayout.HelpBox("Please make sure no list element has Player Input script attached to avoid unexpected behaviors. Manually memove them or just click the 'Update input system' button in the inspector on a list.", MessageType.Info);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Player inputs:", EditorStyles.miniLabel);
                    for (int i = 0; i < playerInputs.Length; i++)
                    {
                        if (i < playerInputs.Length - 1)
                            GUILayout.Label(playerInputs[i].name + ",", EditorStyles.miniLabel);
                        else
                            GUILayout.Label(playerInputs[i].name + "", EditorStyles.miniLabel);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
#endif
            }
        }



        void GetReferences()
        {
            buttonInputProcessorStyle = soTarget.FindProperty("_buttonInputProcessorStyle");
            selectedInputSystemController = soTarget.FindProperty("selectedInputSystemController");
            debugInputs = soTarget.FindProperty("debugInputs");
#if ENABLE_INPUT_SYSTEM
            inputActionAsset = soTarget.FindProperty("inputActionAsset");
#else
            tickRate = soTarget.FindProperty("tickRate");
#endif
        }
    }
}
