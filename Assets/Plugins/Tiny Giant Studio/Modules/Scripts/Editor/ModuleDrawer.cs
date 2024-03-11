using System.Collections.Generic;
using TinyGiantStudio.Layout;
using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Modules
{
    public static class ModuleDrawer
    {
        static GUIStyle headerLabel;
        static Texture addIcon;
        static Texture deleteIcon;


        public static bool ElementUpdatersExist()
        {
            return true;
            //string[] guids = AssetDatabase.FindAssets("t:LayoutElementModule", null);
            //if (guids.Length > 0)
            //    return true;

            //return false;
        }

        public static void UpdateStyles()
        {
            if (addIcon == null)
                addIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Plus.png") as Texture;
            if (deleteIcon == null)
                deleteIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Cross.png") as Texture;


            headerLabel ??= new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                //alignment = TextAnchor.MiddleCenter,
            };
        }

        public static void ElementUpdaterContainerList(string label, string tooltip, LayoutElementModuleContainer elementUpdaterContainer, SerializedProperty serializedProperty, SerializedObject soTarget)
        {
            UpdateStyles();


            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent(label, tooltip), headerLabel);

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(5);

            {
                GUILayout.BeginVertical(EditorStyles.helpBox);


                //GUILayout.BeginVertical("CN EntryBackEven");
                EditorGUI.indentLevel = 0;
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("module"), GUIContent.none, GUILayout.MinWidth(10));
                //GUILayout.Label(GUIContent.none, GUILayout.MaxWidth(5));

                GUILayout.EndHorizontal();

                EditorGUI.indentLevel = 0;
                {
                    if (elementUpdaterContainer.module != null)
                    {
                        if (elementUpdaterContainer.variableHolders != null)
                        {
                            if (elementUpdaterContainer.module.variableHolders != null)
                            {
                                if (elementUpdaterContainer.variableHolders.Length != elementUpdaterContainer.module.variableHolders.Length)
                                {
                                    elementUpdaterContainer.variableHolders = new VariableHolder[elementUpdaterContainer.module.variableHolders.Length];
                                    for (int k = 0; k < elementUpdaterContainer.variableHolders.Length; k++)
                                    {
                                        if (k < elementUpdaterContainer.module.variableHolders.Length)
                                        {
                                            elementUpdaterContainer.variableHolders[k] = elementUpdaterContainer.module.variableHolders[k];
                                        }
                                    }

                                    soTarget.Update();
                                }
                            }

                            for (int j = 0; j < elementUpdaterContainer.variableHolders.Length; j++)
                            {
                                DrawVariableHolder(elementUpdaterContainer, serializedProperty, j);
                            }

                            string warning = elementUpdaterContainer.module.VariableWarnings(elementUpdaterContainer.variableHolders);
                            if (warning != null)
                            {
                                if (warning.Length > 0)
                                {
                                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                                }
                            }
                        }
                    }
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
        public static void BaseModuleContainerList(string label, string tooltip, List<ModuleContainer> moduleContainers, SerializedProperty serializedProperty, SerializedObject soTarget, SerializedProperty boolProperty = null)
        {
            UpdateStyles();


            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginVertical(EditorStyles.toolbar);

            GUILayout.BeginHorizontal();

            if (boolProperty != null)
            {
                EditorGUILayout.PropertyField(boolProperty, GUIContent.none, GUILayout.MaxWidth(17));
            }

            EditorGUILayout.LabelField(new GUIContent(label, tooltip), headerLabel);

            if (GUILayout.Button(addIcon, EditorStyles.toolbarButton, GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
                EmptyEffect(moduleContainers, serializedProperty);

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(5);

            for (int i = 0; i < moduleContainers.Count; i++)
            {
                if (serializedProperty.arraySize <= i) //no module
                    continue;

                GUILayout.BeginVertical(EditorStyles.helpBox);


                //GUILayout.BeginVertical("CN EntryBackEven");
                EditorGUI.indentLevel = 0;
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("module"), GUIContent.none, GUILayout.MinWidth(10));
                GUILayout.Label(GUIContent.none, GUILayout.MaxWidth(5));

                if (GUILayout.Button(deleteIcon, EditorStyles.toolbarButton, GUILayout.MinHeight(20), GUILayout.MaxWidth(20)))
                {
                    if (!Application.isPlaying)
                        Undo.RecordObject(serializedProperty.serializedObject.targetObject, "Delete module");

                    serializedProperty.DeleteArrayElementAtIndex(i);
                }

                GUILayout.EndHorizontal();

                EditorGUI.indentLevel = 0;
                if (i < moduleContainers.Count)
                {
                    if (moduleContainers[i].module != null)
                    {
                        if (moduleContainers[i].variableHolders != null)
                        {
                            if (moduleContainers[i].module.variableHolders != null)
                            {
                                if (moduleContainers[i].variableHolders.Length != moduleContainers[i].module.variableHolders.Length)
                                {
                                    moduleContainers[i].variableHolders = new VariableHolder[moduleContainers[i].module.variableHolders.Length];
                                    for (int k = 0; k < moduleContainers[i].variableHolders.Length; k++)
                                    {
                                        if (k < moduleContainers[i].variableHolders.Length)
                                        {
                                            moduleContainers[i].variableHolders[k] = moduleContainers[i].module.variableHolders[k];
                                        }
                                    }
                                    soTarget.Update();
                                }
                            }

                            for (int j = 0; j < moduleContainers[i].variableHolders.Length; j++)
                            {
                                DrawVariableHolder(moduleContainers, serializedProperty, i, j);
                            }

                            string warning = moduleContainers[i].module.VariableWarnings(moduleContainers[i].variableHolders);
                            if (warning != null)
                            {
                                if (warning.Length > 0)
                                {
                                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                                }
                            }
                        }
                    }
                }

                GUILayout.EndVertical();
                if (i + 1 != moduleContainers.Count)
                {
                    EditorGUILayout.Space(5);
                }
            }

            GUILayout.EndVertical();
        }

        public static void EmptyEffect(List<ModuleContainer> moduleList, SerializedProperty serializedProperty)
        {
            if (!Application.isPlaying)
                Undo.RecordObject(serializedProperty.serializedObject.targetObject, "Add module");
            serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
        }


        static void DrawVariableHolder(LayoutElementModuleContainer elementUpdater, SerializedProperty serializedProperty, int j)
        {
            if (elementUpdater.module.variableHolders != null)
            {
                if (!ShowProperty(elementUpdater.module.variableHolders, j, elementUpdater.variableHolders))
                    return;



                string contentName = string.Empty;
                string contentTooltip = string.Empty;

                if (elementUpdater.module.variableHolders[j].variableName != null)
                    if (elementUpdater.module.variableHolders[j].variableName != string.Empty)
                        contentName = elementUpdater.module.variableHolders[j].variableName;

                if (string.IsNullOrEmpty(contentName))
                    contentName = "Unlabeled variable";

                if (elementUpdater.module.variableHolders[j].tooltip != null)
                    if (elementUpdater.module.variableHolders[j].tooltip != string.Empty)
                        contentTooltip = elementUpdater.module.variableHolders[j].tooltip;


                GUIContent variableLabelContent = new GUIContent(contentName, contentTooltip);

                ModuleVariableType type = elementUpdater.module.variableHolders[j].type;

                SerializedProperty property;
                string propertyName = ModuleDrawer.GetPropertyName(type);

                if (elementUpdater != null)
                {
                    if (serializedProperty.FindPropertyRelative("variableHolders").arraySize > j)
                    {
                        property = serializedProperty.FindPropertyRelative("variableHolders").GetArrayElementAtIndex(j).FindPropertyRelative(propertyName);
                        EditorGUILayout.PropertyField(property, variableLabelContent);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Problem");
                    }
                }
            }
        }

        static void DrawVariableHolder(List<ModuleContainer> moduleContainers, SerializedProperty serializedContainer, int i, int j)
        {
            if (moduleContainers[i].module.variableHolders != null)
            {
                if (!ShowProperty(moduleContainers[i].module.variableHolders, j, moduleContainers[i].variableHolders))
                    return;



                string contentName = string.Empty;
                string contentTooltip = string.Empty;

                if (moduleContainers[i].module.variableHolders[j].variableName != null)
                    if (moduleContainers[i].module.variableHolders[j].variableName != string.Empty)
                        contentName = moduleContainers[i].module.variableHolders[j].variableName;

                if (string.IsNullOrEmpty(contentName))
                    contentName = "Unlabeled variable";

                if (moduleContainers[i].module.variableHolders[j].tooltip != null)
                    if (moduleContainers[i].module.variableHolders[j].tooltip != string.Empty)
                        contentTooltip = moduleContainers[i].module.variableHolders[j].tooltip;


                GUIContent variableLabelContent = new GUIContent(contentName, contentTooltip);

                ModuleVariableType type = moduleContainers[i].module.variableHolders[j].type;

                SerializedProperty property;
                string propertyName = ModuleDrawer.GetPropertyName(type);

                if (moduleContainers != null)
                {
                    if (serializedContainer.arraySize > i)
                    {
                        if (serializedContainer.GetArrayElementAtIndex(i).FindPropertyRelative("variableHolders").arraySize > j)
                        {
                            property = serializedContainer.GetArrayElementAtIndex(i).FindPropertyRelative("variableHolders").GetArrayElementAtIndex(j).FindPropertyRelative(propertyName);
                            EditorGUILayout.PropertyField(property, variableLabelContent);
                        }
                    }
                }
            }
        }

        //should check from the module in module container list
        static bool ShowProperty(VariableHolder[] moduleVariables, int i, VariableHolder[] textVariables)
        {
            if (moduleVariables[i].hideIf == null)
                return true;

            if (!string.IsNullOrEmpty(moduleVariables[i].hideIf))
            {
                for (int j = 0; j < moduleVariables.Length; j++)
                {
                    if (j == i)
                        continue;

                    if (moduleVariables[j].type == ModuleVariableType.@bool)
                    {
                        if (moduleVariables[j].variableName == moduleVariables[i].hideIf)
                        {
                            if (textVariables[j] == null)
                                return true;

                            if (textVariables[j].boolValue == true)
                                return false;
                            else
                                return true;
                        }
                    }
                }
            }
            return true;
        }



        public static string GetPropertyName(ModuleVariableType type)
        {
            string propertyName;
            if (type == ModuleVariableType.@float)
                propertyName = "floatValue";
            else if (type == ModuleVariableType.@int)
                propertyName = "intValue";
            else if (type == ModuleVariableType.@bool)
                propertyName = "boolValue";
            else if (type == ModuleVariableType.@string)
                propertyName = "stringValue";
            else if (type == ModuleVariableType.vector2)
                propertyName = "vector2Value";
            else if (type == ModuleVariableType.vector3)
                propertyName = "vector3Value";
            else if (type == ModuleVariableType.animationCurve)
                propertyName = "animationCurve";
            else if (type == ModuleVariableType.gameObject)
                propertyName = "gameObjectValue";
            else if (type == ModuleVariableType.physicMaterial)
                propertyName = "physicMaterialValue";
            else
                propertyName = "floatValue";
            return propertyName;
        }
    }
}