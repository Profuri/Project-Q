using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.PackageImporter
{
    [CustomEditor(typeof(TGS_Asset))]
    public class TGS_AssetEditor : Editor
    {
        TGS_Asset myTarget;
        SerializedObject soTarget;

        SerializedProperty assetName;
        SerializedProperty publisher;
        SerializedProperty packages;

        GUIStyle headerLabel = null;
        GUIStyle groupLabel = null;
        GUIStyle packageNameStyle = null;
        GUIStyle buttonStyle = null;



        void OnEnable()
        {
            myTarget = (TGS_Asset)target;
            soTarget = new SerializedObject(target);

            GetReferences();
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            GenerateStyle();

            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel = 0;
            EditorGUILayout.PropertyField(assetName);
            EditorGUILayout.PropertyField(publisher);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Packages: " + myTarget.packages.Count, headerLabel, GUILayout.MinWidth(120));
            if (GUILayout.Button("Add new package", buttonStyle, GUILayout.MaxWidth(120), GUILayout.MinWidth(120)))
            {
                AddNewPackage();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            for (int i = 0; i < myTarget.packages.Count; i++)
            {
                DrawPackage(i);
                if (i + 1 < myTarget.packages.Count)
                    GUILayout.Space(15);
            }


            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                EditorUtility.SetDirty(myTarget);
            }
        }

        private void RedrawWindow()
        {
            if (EditorWindow.HasOpenInstances<TGS_PackageImporterWindow>())
            {
                var window = EditorWindow.GetWindow<TGS_PackageImporterWindow>();
                window.Close();
                TGS_PackageImporterWindow.ShowWindow();
            }
        }

        private void AddNewPackage()
        {
            myTarget.packages.Add(new TGS_Package());
            RedrawWindow();
        }

        void DrawPackage(int i)
        {
            if (packages.arraySize <= i)
                return;
            if (myTarget.packages.Count <= i)
                return;
            if (myTarget.packages[i] == null)
                return;

            if (i % 2f == 0)
                GUI.color = new Color(0.97f, 0.97f, 1f, 1);
            else
                GUI.color = new Color(0.97f, 1, 0.97f, 1);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            float defaultWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 40;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(new GUIContent("Name", "User friendly name shown in the importer"), packageNameStyle, GUILayout.MaxWidth(40));
            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("packageName"), GUIContent.none);
            if (GUILayout.Button("Remove", buttonStyle, GUILayout.MinWidth(60), GUILayout.MaxWidth(60)))
            {
                myTarget.packages.RemoveAt(i);
                Repaint();
                RedrawWindow();
                return;
            }
            GUILayout.EndHorizontal();

            EditorGUI.indentLevel = 1;
            EditorGUIUtility.labelWidth = 90;
            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("package"), new GUIContent("Package", ""));
            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("Description"), new GUIContent("Description", "Description shown in the importer"));
            EditorGUIUtility.labelWidth = 150;
            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("packageIdentifier"), new GUIContent("Package Indentifier", "The ID used to identify this package. This must be unique."));


            GUILayout.Space(5);
            PipelineSettings(i);

            EditorGUI.indentLevel = 0;

            GUILayout.Space(5);
            UnityVersionSettings(i);

            DebugInfo(i);


            GUILayout.Space(5);
            GUILayout.EndVertical();
            EditorGUIUtility.labelWidth = defaultWidth;

        }
        void DebugInfo(int i)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.indentLevel = 2;
            GUILayout.Label(new GUIContent("Package version info", ""), groupLabel);
            GUILayout.EndHorizontal();

            EditorGUI.indentLevel = 1;
            EditorGUIUtility.labelWidth = 170;
            if (myTarget.packages[i].currentVersion == 0)
                myTarget.packages[i].currentVersion = 1;
            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("currentVersion"), new GUIContent("Current package version", ""));
            EditorGUILayout.LabelField("Last Installed Version: " + myTarget.packages[i].GetLastInstalledVersion());
            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("ignoreUpdates"), new GUIContent("Ignore updates", ""));
        }

        void UnityVersionSettings(int i)
        {
            GUILayout.Label(new GUIContent("Unity version check not implemented fully yet."), groupLabel, GUILayout.MinWidth(60)); //TODO
            EditorGUI.indentLevel = 0;
            //GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("hasUnityVersionDependancy"), GUIContent.none, GUILayout.MaxWidth(20));
            GUILayout.Label(new GUIContent("Unity version", "The tick mark controls if there is dependancy to unity version or not."), groupLabel, GUILayout.MinWidth(60));
            GUILayout.EndHorizontal();

            if (!myTarget.packages[i].hasUnityVersionDependancy)
                return;

            EditorGUIUtility.labelWidth = 25;

            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("minimumUnityVersion"), new GUIContent("Min", "Minimum Unity Version. 0 = any version."), GUILayout.MinWidth(35));
            if (myTarget.packages[i].minimumUnityVersion == 0)
                GUILayout.Label(new GUIContent("(Any)", "Version 0 is used as any version."), GUILayout.MaxWidth(35));

            GUILayout.Label(new GUIContent("|", ""), GUILayout.MaxWidth(10));
            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("maximumUnityVersion"), new GUIContent("Max", "Maximum Unity version. 0 = any version."), GUILayout.MinWidth(35));
            if (myTarget.packages[i].maximumUnityVersion == 0)
                GUILayout.Label(new GUIContent("(Any)", "Version 0 is used as any version."), GUILayout.MaxWidth(35));

            GUILayout.EndHorizontal();
        }

        private void PipelineSettings(int i)
        {
            EditorGUI.indentLevel = 0;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("hasPipelineDependancy"), GUIContent.none, GUILayout.MaxWidth(20));
            GUILayout.Label(new GUIContent("Pipeline", "The tick mark controls if there is dependancy to pipeline or not."), groupLabel, GUILayout.MinWidth(60));

            if (myTarget.packages[i].hasPipelineDependancy)
            {
                EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("targetPipeline"), GUIContent.none);
            }
            GUILayout.EndHorizontal();

            if (!myTarget.packages[i].hasPipelineDependancy || myTarget.packages[i].targetPipeline == TGS_Package.Pipeline.SRP)
                return;


            GUILayout.BeginHorizontal();

            if (myTarget.packages[i].minimumPipelineVersion.Length != 2)
            {
                myTarget.packages[i].minimumPipelineVersion = new int[] { 0, 0 };
                Repaint();
            }
            if (myTarget.packages[i].maximumPipelineVersion.Length != 2)
            {
                myTarget.packages[i].maximumPipelineVersion = new int[] { 0, 0 };
                Repaint();
            }

            //EditorGUILayout.PropertyField(packages.GetArrayElementAtIndex(i).FindPropertyRelative("minimumPipelineVersion"), new GUIContent("Minimum", "Minimum pipeline version. 0 = any version."));

            EditorGUIUtility.labelWidth = 110;
            GUILayout.Label(new GUIContent("Minimum", "Minimum pipeline version. \n0 = any version."), GUILayout.MinWidth(25), GUILayout.MaxWidth(56));
            myTarget.packages[i].minimumPipelineVersion[0] = EditorGUILayout.IntField(myTarget.packages[i].minimumPipelineVersion[0], GUILayout.MinWidth(36), GUILayout.MaxWidth(70));
            if (myTarget.packages[i].minimumPipelineVersion[0] == 0)
                GUILayout.Label(new GUIContent("(Any)", "Version 0 is used as any version."), GUILayout.MaxWidth(40));
            else
            {
                GUILayout.Label(new GUIContent(".", "Minor version"), GUILayout.MaxWidth(7));
                EditorGUIUtility.labelWidth = 1;
                myTarget.packages[i].minimumPipelineVersion[1] = EditorGUILayout.IntField(".", myTarget.packages[i].minimumPipelineVersion[1], GUILayout.MinWidth(15), GUILayout.MaxWidth(40));
            }

            GUILayout.FlexibleSpace();

            EditorGUIUtility.labelWidth = 110;
            GUILayout.Label(new GUIContent("Maximum", "Maximum pipeline version. \n0 = any version."), GUILayout.MinWidth(26), GUILayout.MaxWidth(56));
            myTarget.packages[i].maximumPipelineVersion[0] = EditorGUILayout.IntField(myTarget.packages[i].maximumPipelineVersion[0], GUILayout.MinWidth(36), GUILayout.MaxWidth(70));
            if (myTarget.packages[i].maximumPipelineVersion[0] == 0)
                GUILayout.Label(new GUIContent("(Any)", "Version 0 is used as any version."), GUILayout.MaxWidth(40));
            else
            {
                GUILayout.Label(new GUIContent(".", "Minor version"), GUILayout.MaxWidth(7));
                EditorGUIUtility.labelWidth = 1;
                myTarget.packages[i].maximumPipelineVersion[1] = EditorGUILayout.IntField(".", myTarget.packages[i].maximumPipelineVersion[1], GUILayout.MinWidth(15), GUILayout.MaxWidth(40));
            }

            GUILayout.EndHorizontal();
        }

        void GetReferences()
        {
            packages = soTarget.FindProperty("packages");
            assetName = soTarget.FindProperty("assetName");
            publisher = soTarget.FindProperty("publisher");
        }

        void GenerateStyle()
        {
            headerLabel ??= new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleLeft,
            };
            groupLabel ??= new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
            };
            packageNameStyle ??= new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            buttonStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontStyle = FontStyle.Italic,
            };

            packageNameStyle.normal.textColor = Color.yellow;
        }
    }
}