using TinyGiantStudio.EditorHelpers;
using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(RaycastSelector))]
    public class RaycastSelectorEditor : Editor
    {
        RaycastSelector myTarget;
        SerializedObject soTarget;

        SerializedProperty UILayer;
        SerializedProperty maxRayDistance;

        SerializedProperty onlyOneTargetFocusedAtOnce;
        SerializedProperty unselectBtnOnUnhover;

        void OnEnable()
        {
            myTarget = (RaycastSelector)target;
            soTarget = new SerializedObject(target);

            GetReferences();
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            //GenerateStyle();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space(5);

            //MText_Editor_Methods.ItalicHorizontalField(myCamera, "Camera", "If not assigned, it will automatically get Camera.main on Start", FieldSize.tiny);
            MText_Editor_Methods.ItalicHorizontalField(UILayer, "Layer", "Which layers should this raycaster check." +
                "\nIt's a good habbit to have a specific layer for UI, for both performance and organizing.", FieldSize.small);
            MText_Editor_Methods.ItalicHorizontalField(maxRayDistance, "Max Ray Distance", "The max distance the raycast check needs to be done.\n" +
                "It's more efficient to use the shortest distance possible.", FieldSize.large);

            EditorGUILayout.Space(5);
            MText_Editor_Methods.ItalicHorizontalField(onlyOneTargetFocusedAtOnce, "Single focus", "onlyOneTargetFocusedAtOnce True = How normal UI works. It toggles if clicking a inputfield enables it and clicking somewhere else disables it", FieldSize.extraLarge);
            MText_Editor_Methods.ItalicHorizontalField(unselectBtnOnUnhover, "Unselect On Unhover", "Unhovering mouse from a Btn will unselect it", FieldSize.extraLarge);

            EditorGUILayout.Space(5);
            Warnings();

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                EditorUtility.SetDirty(myTarget);
            }
        }

        private void Warnings()
        {
            if (!myTarget.GetComponent<RaycastInputProcessor>())
            {
                EditorGUILayout.HelpBox("No input processor found. If you are using a custom solution, ignore this warning. Otherwise, please add Raycast Selector Input Processor", MessageType.Warning);
                if (GUILayout.Button("Add Raycast Selector Input Processor"))
                {
                    myTarget.gameObject.AddComponent<RaycastInputProcessor>();
                    EditorUtility.SetDirty(myTarget.gameObject);
                }
            }
        }

        void GetReferences()
        {
            UILayer = soTarget.FindProperty("UILayer");
            maxRayDistance = soTarget.FindProperty("maxRayDistance");

            onlyOneTargetFocusedAtOnce = soTarget.FindProperty("onlyOneTargetFocusedAtOnce");
            unselectBtnOnUnhover = soTarget.FindProperty("unselectBtnOnUnhover");
        }
    }
}