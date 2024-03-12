using TinyGiantStudio.EditorHelpers;
using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(RaycastInputProcessor))]
    public class RaycastSelectorInputProcessorEditor : Editor
    {
        RaycastInputProcessor myTarget;
        SerializedObject soTarget;

        SerializedProperty myCamera;
        SerializedProperty pointerOnUI;
        SerializedProperty currentTarget;




        void OnEnable()
        {
            myTarget = (RaycastInputProcessor)target;
            soTarget = new SerializedObject(target);

            GetReferences();
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            //GenerateStyle();
            EditorGUI.BeginChangeCheck();

            MText_Editor_Methods.ItalicHorizontalField(myCamera, "Camera", "If not assigned, it will automatically get Camera.main on Start", FieldSize.small);
            if (Application.isPlaying)
            {
                GUI.enabled = false;
                MText_Editor_Methods.ItalicHorizontalField(pointerOnUI, "Pointer On", "", FieldSize.normal);
                MText_Editor_Methods.ItalicHorizontalField(currentTarget, "Current Target", "", FieldSize.normal);
                GUI.enabled = true;
            }


            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                EditorUtility.SetDirty(myTarget);
            }
        }
        void GetReferences()
        {
            myCamera = soTarget.FindProperty("myCamera");
            pointerOnUI = soTarget.FindProperty("pointerOnUI");
            currentTarget = soTarget.FindProperty("currentTarget");
        }
    }
}
