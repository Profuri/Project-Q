using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Modules
{
    [CustomEditor(typeof(ModuleCore), true)]
    public class ModuleEditor : Editor
    {
        ModuleCore myTarget;
        SerializedObject soTarget;

        SerializedProperty variableHolders;
        Texture deleteIcon;
        GUIStyle defaultLabel = null;
        Vector2 scrollPos;

        void OnEnable()
        {
            myTarget = (ModuleCore)target;
            soTarget = new SerializedObject(target);

            variableHolders = soTarget.FindProperty("variableHolders");

            deleteIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Cross.png") as Texture;
        }

        public override void OnInspectorGUI()
        {
            GenerateStyle();
            soTarget.Update();
            EditorGUI.BeginChangeCheck();

            if (myTarget.variableHolders != null)
            {
                EditorGUILayout.HelpBox("Modifiable variables by text. These are dependent on individual modules. Changing them might result in undesirable effects. If that happens, please fix them from the instruction in code summary or download the original version from the asset store.", MessageType.Warning);

                int indexWidth = 16;
                int nameWidth = 70;
                int typeMinWidth = 50;
                int typeMaxWidth = 125;
                int defaultValueWidth = 75;
                int tooltipWidth = 75;
                int hideIfWidth = 70;

                scrollPos =
            EditorGUILayout.BeginScrollView(scrollPos);

                if (myTarget.variableHolders.Length > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", defaultLabel, GUILayout.MaxWidth(indexWidth));//Index

                    EditorGUILayout.LabelField("Name", defaultLabel, GUILayout.MinWidth(nameWidth));
                    EditorGUILayout.LabelField("Type", defaultLabel, GUILayout.MinWidth(typeMinWidth), GUILayout.MaxWidth(typeMaxWidth));
                    EditorGUILayout.LabelField(new GUIContent("Hide if", "The variable will be invisible if the bool variable with show if name is set to true."), defaultLabel, GUILayout.MinWidth(hideIfWidth), GUILayout.MaxWidth(hideIfWidth * 2));
                    EditorGUILayout.LabelField(new GUIContent("defaultValue"), defaultLabel, GUILayout.MinWidth(defaultValueWidth), GUILayout.MaxWidth(defaultValueWidth * 2));
                    EditorGUILayout.LabelField(new GUIContent("Tooltip", "Little info boxes like this one."), defaultLabel, GUILayout.MinWidth(tooltipWidth), GUILayout.MaxWidth(tooltipWidth * 2));
                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(20), GUILayout.MinWidth(20));
                    EditorGUILayout.EndHorizontal();
                }

                for (int i = 0; i < myTarget.variableHolders.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    Color color = GUI.color;
                    GUI.color = Color.gray;
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.MaxWidth(indexWidth)); //Index
                    GUI.color = color;

                    EditorGUILayout.PropertyField(variableHolders.GetArrayElementAtIndex(i).FindPropertyRelative("variableName"), GUIContent.none, GUILayout.MinWidth(nameWidth));
                    EditorGUILayout.PropertyField(variableHolders.GetArrayElementAtIndex(i).FindPropertyRelative("type"), GUIContent.none, GUILayout.MinWidth(typeMinWidth), GUILayout.MaxWidth(typeMaxWidth));
                    EditorGUILayout.PropertyField(variableHolders.GetArrayElementAtIndex(i).FindPropertyRelative("hideIf"), GUIContent.none, GUILayout.MinWidth(hideIfWidth), GUILayout.MaxWidth(hideIfWidth * 2));
                    EditorGUILayout.PropertyField(variableHolders.GetArrayElementAtIndex(i).FindPropertyRelative(ModuleDrawer.GetPropertyName(myTarget.variableHolders[i].type)), GUIContent.none, GUILayout.MinWidth(defaultValueWidth), GUILayout.MaxWidth(defaultValueWidth * 2));
                    EditorGUILayout.PropertyField(variableHolders.GetArrayElementAtIndex(i).FindPropertyRelative("tooltip"), GUIContent.none, GUILayout.MinWidth(tooltipWidth), GUILayout.MaxWidth(tooltipWidth * 2));

                    if (GUILayout.Button(deleteIcon, EditorStyles.label, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
                    {
                        RemoveVariable(i);
                    }

                    GUI.color = color;
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(10);

                if (GUILayout.Button("Add new variable", GUILayout.MaxHeight(25)))
                {
                    System.Array.Resize(ref myTarget.variableHolders, myTarget.variableHolders.Length + 1);
                    EditorUtility.SetDirty(myTarget);
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                myTarget.variableHolders = new VariableHolder[0];
            }

            DrawDefaultInspector();


            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
            }
        }

        void RemoveVariable(int removeTarget)
        {
            for (int i = removeTarget; i < myTarget.variableHolders.Length - 1; i++)
            {
                myTarget.variableHolders[i] = myTarget.variableHolders[i + 1];
            }
            System.Array.Resize(ref myTarget.variableHolders, myTarget.variableHolders.Length - 1);
            EditorUtility.SetDirty(myTarget);
        }

        void GenerateStyle()
        {
            if (defaultLabel == null)
            {
                defaultLabel = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    fontStyle = FontStyle.BoldAndItalic,
                    fontSize = 13
                };
                defaultLabel.normal.textColor = Color.yellow;
            }
        }
    }
}