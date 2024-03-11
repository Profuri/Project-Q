using TinyGiantStudio.EditorHelpers;
using UnityEditor;
using UnityEngine;


namespace TinyGiantStudio.Layout
{
    [CustomEditor(typeof(LayoutElement))]
    public class LayoutElementEditor : Editor
    {
        LayoutElement myTarget;
        SerializedObject soTarget;

        SerializedProperty autoCalculateSize;

        SerializedProperty width;
        SerializedProperty height;

        SerializedProperty xOffset;
        SerializedProperty yOffset;
        SerializedProperty zOffset;

        SerializedProperty ignoreElement;
        SerializedProperty lineBreak;
        SerializedProperty space;


        void OnEnable()
        {
            myTarget = (LayoutElement)target;
            soTarget = new SerializedObject(target);

            FindProperties();
        }
        public override void OnInspectorGUI()
        {
            soTarget.Update();
            GenerateStyle();
            EditorGUI.BeginChangeCheck();

            if (myTarget.ignoreElement || myTarget.lineBreak || myTarget.space)
                GUI.enabled = false;

            MText_Editor_Methods.ItalicHorizontalField(autoCalculateSize, "Auto Calculate Size", "", FieldSize.large);
            GUILayout.Space(10);

            if (myTarget.autoCalculateSize)
                GUI.enabled = false;

            GUILayout.BeginHorizontal();
            MText_Editor_Methods.HorizontalField(height, "Height", "", FieldSize.tiny);
            GUILayout.FlexibleSpace();
            MText_Editor_Methods.HorizontalField(width, "Width", "", FieldSize.tiny);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.enabled = true;
            if (myTarget.ignoreElement || myTarget.lineBreak || myTarget.space)
                GUI.enabled = false;

            GUILayout.BeginVertical();
            GUILayout.Label("Offset");
            EditorGUI.indentLevel = 2;
            MText_Editor_Methods.HorizontalField(xOffset, "X", "", FieldSize.tiny);
            MText_Editor_Methods.HorizontalField(yOffset, "Y", "", FieldSize.tiny);
            MText_Editor_Methods.HorizontalField(zOffset, "Z", "", FieldSize.tiny);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            EditorGUI.indentLevel = 0;
            GUI.enabled = true;
            MText_Editor_Methods.HorizontalField(ignoreElement, "Ignore Element", "Ignores this element in layout group.", FieldSize.extraLarge);
            if (myTarget.ignoreElement)
                GUI.enabled = false;
            MText_Editor_Methods.HorizontalField(lineBreak, "LineBreak", "Used in Grid layout.\nEnds current line and moves everything after it to next one.", FieldSize.extraLarge);
            MText_Editor_Methods.HorizontalField(space, "Space", "Used in Grid layout.", FieldSize.extraLarge);


            if (EditorGUI.EndChangeCheck())
            {
                if (soTarget.ApplyModifiedProperties())
                {
                    EditorUtility.SetDirty(myTarget);
                }
            }
        }

        void GenerateStyle()
        {

        }

        void FindProperties()
        {
            autoCalculateSize = soTarget.FindProperty("autoCalculateSize");

            width = soTarget.FindProperty("width");
            height = soTarget.FindProperty("height");

            xOffset = soTarget.FindProperty("xOffset");
            yOffset = soTarget.FindProperty("yOffset");
            zOffset = soTarget.FindProperty("zOffset");

            ignoreElement = soTarget.FindProperty("ignoreElement");
            lineBreak = soTarget.FindProperty("lineBreak");
            space = soTarget.FindProperty("space");
        }
    }
}