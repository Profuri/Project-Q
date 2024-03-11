using System.Collections.Generic;
using TinyGiantStudio.EditorHelpers;
using TinyGiantStudio.Layout;
using TinyGiantStudio.Modules;
using UnityEditor;
using UnityEditor.AnimatedValues;

using UnityEngine;

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(Modular3DText))]
    public class Modular3DTextEditor : Editor
    {
        private Modular3DText myTarget;
        private SerializedObject soTarget;

        //SerializedProperty text;

        //main settings
        private SerializedProperty font;

        private SerializedProperty material;
        private SerializedProperty fontSize;

        private SerializedProperty autoLetterSize;
        private SerializedProperty _wordSpacing;

        //effects
        private SerializedProperty useModules;

        private SerializedProperty startAddingModuleFromChar;
        private SerializedProperty addingModules;
        private SerializedProperty startDeletingModuleFromChar;
        private SerializedProperty deletingModules;
        private SerializedProperty customDeleteAfterDuration;
        private SerializedProperty deleteAfter;
        private SerializedProperty applyModuleOnNewCharacter;
        private SerializedProperty applyModulesOnStart;
        private SerializedProperty applyModulesOnEnable;

        //advanced settings
        private SerializedProperty destroyChildObjectsWithGameObject;

        private SerializedProperty repositionOldCharacters;
        private SerializedProperty reApplyModulesToOldCharacters;
        private SerializedProperty hideOverwrittenVariablesFromInspector;
        private SerializedProperty combineMeshInEditor;
        private SerializedProperty singleInPrefab;
        private SerializedProperty combineMeshDuringRuntime;
        private SerializedProperty hideLettersInHierarchyInPlayMode;
        private SerializedProperty updateTextOncePerFrame;
        private SerializedProperty autoSaveMesh;
        private SerializedProperty canBreakOutermostPrefab;

        private SerializedProperty debugLogs;
        private SerializedProperty runTimeLogging;
        private SerializedProperty editorTimeLogging;
        private SerializedProperty logTextUpdates;
        private SerializedProperty logFontUpdates;
        private SerializedProperty logMaterialUpdates;
        private SerializedProperty logDeletedCharacters;
        private SerializedProperty logSingleMeshStatus;

        private SerializedProperty generatedMeshes;

        private SerializedProperty meshPostProcess;
        private SerializedProperty useIncreasedVerticiesCountForCombinedMesh;

        //Debug -- starts
        private SerializedProperty wordArray;

        //Debug --- ends

        #region Tool-tips

        private readonly string addingtoolTip = "During runtime, these modules are called when new characters are added to the text. \nThis behavior can be modified.";
        private readonly string deleteingtoolTip = "During runtime, these modules are called when characters are removed from the text.";
        private readonly string modulesToolTip = "Modules are drag-and-drop scriptable objects used to animate/manipulate texts or other 3D UI elements. They are only applied during run time.";

        #endregion Tool-tips

        private AnimBool showMainSettingsInEditor;
        private AnimBool showLayoutSettingsInEditor;
        private AnimBool showModuleSettingsInEditor;
        private AnimBool showModuleRunSettingsInEditor;
        private AnimBool showAdvancedSettingsInEditor;
        private AnimBool showAdvancedInspectorSettingsInEditor;
        private AnimBool showAdvancedBehaviorSettingsInEditor;
        private AnimBool showDebugSettingsInEditor;

        //style
        private GUIStyle textInputStyle = null;

        private GUIStyle toggleStyle = null;
        private GUIStyle foldOutStyle = null;
        private GUIStyle defaultLabel = null;
        private GUIStyle defaultMultilineLabel = null;

        #region colors

        private Color openedFoldoutTitleColor = new Color(136 / 255f, 173 / 255f, 234 / 255f, 1f);
        private static readonly Color openedFoldoutTitleColorDarkSkin = new Color(136 / 255f, 173 / 255f, 234 / 255f, 1f);
        private static readonly Color openedFoldoutTitleColorLightSkin = new Color(31 / 255f, 31 / 255f, 31 / 255f, 1);

        /// <summary>
        /// settings that are turned off but still visible. Not the toggle button's color
        /// </summary>
        private static readonly Color toggledOffColor = new Color(0.75f, 0.75f, 0.75f);

        private static readonly Color toggledOnButtonColor = Color.yellow;
        private static readonly Color toggledOffButtonColor = Color.white;

        #endregion colors

        public AssetSettings settings;

        private Texture documentationIcon;
        private bool inheritsStyleFromParents;
        private Color originalBackgroundColor;

        private readonly float iconSize = 20;
        private readonly string layoutDocumentationURL = "https://ferdowsur.gitbook.io/layout-system/layout-group";

        private readonly GUIContent combineLabel = new GUIContent("Single mesh", "Combines each letter into a single mesh.");
        private readonly GUIContent smallCase = new GUIContent("ab", "Lower case. \n\nProperty Name: 'LowerCase'");
        private readonly GUIContent capitalize = new GUIContent("AB", "UPPER CASE. \n\nProperty Name: 'Capitalize'");

        private readonly string variableName = "\n\nVariable name: ";

        //for grid layout
        private Alignment anchor;

        private Vector3 spacing;
        private float width;
        private float height;

        private void OnEnable()
        {
            myTarget = (Modular3DText)target;
            soTarget = new SerializedObject(target);

            FindProperties();
            LoadFoldoutValues();

            documentationIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Documentation.png") as Texture;

            if (!settings)
                settings = StaticMethods.VerifySettings(settings);

            if (EditorGUIUtility.isProSkin)
            {
                if (settings)
                    openedFoldoutTitleColor = settings.openedFoldoutTitleColor_darkSkin;
                else
                    openedFoldoutTitleColor = openedFoldoutTitleColorDarkSkin;
            }
            else
            {
                if (settings)
                    openedFoldoutTitleColor = settings.openedFoldoutTitleColor_lightSkin;
                else
                    openedFoldoutTitleColor = openedFoldoutTitleColorLightSkin;
            }

            inheritsStyleFromParents = myTarget.DoesStyleInheritFromAParent();
            originalBackgroundColor = GUI.backgroundColor;
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            GenerateStyle();

            EditorGUI.BeginChangeCheck();

            WarningCheck();

            myTarget.Text = EditorGUILayout.TextArea(myTarget.Text, textInputStyle, GUILayout.Height(70));

            GUILayout.Space(5);

            MainSettings();

            GUILayout.Space(5);

            if (showLayoutSettingsInEditor.faded != 0)
                LayoutSettings(out anchor, out spacing, out width, out height); //todo: refactor
            else
                LayoutSettings();

            GUILayout.Space(5);

            ModuleSettings();

            GUILayout.Space(5);

            AdvancedSettings(); //----------------

            GUILayout.Space(5);

            DebugView(); //----------------

            if (EditorGUI.EndChangeCheck())
            {
                Font font = myTarget.Font;
                string text = myTarget.Text;

                if (showLayoutSettingsInEditor.faded != 0)
                    ApplyGridLayoutSettings(anchor, spacing, width, height);

                //In prefab mode font change wasn't updating for some reasons
                if (font != myTarget.Font || soTarget.ApplyModifiedProperties())
                {
                    if (text == myTarget.Text)
                        myTarget.oldText = "";

                    myTarget.updatedAfterStyleUpdateOnPrefabInstances = false;
                    myTarget.UpdateText();
                }

                EditorUtility.SetDirty(myTarget);
            }
        }

        #region Primary Sections

        private void MainSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showMainSettingsInEditor.target = EditorGUILayout.Foldout(showMainSettingsInEditor.target, "Main Settings", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showMainSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 0;

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                MText_Editor_Methods.ItalicHorizontalField(font, "Font", "Modular 3D Text Font. \nIt's a property. So, text is auotmatically updated if it is changed." + variableName + "Font", FieldSize.tiny);

                bool showControllableVariables = !inheritsStyleFromParents || !myTarget.hideOverwrittenVariablesFromInspector;

                if (showControllableVariables)
                    MText_Editor_Methods.ItalicHorizontalField(fontSize, "Size", "Assigning a new font size recreates the entire text. This is to avoid interfering with anything any module or usercreated code is doing." + variableName + "FontSize", FieldSize.tiny);

                GUILayout.EndVertical();

                if (showControllableVariables)
                {
                    GUILayout.BeginVertical(GUILayout.MaxWidth(50));

                    if (myTarget.Material)
                    {
                        Texture2D texture = AssetPreview.GetAssetPreview(myTarget.Material);
                        if (texture)
                        {
                            try { GUILayout.Box(texture, GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(40)); }
                            catch { }
                        }
                    }

                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                if (showControllableVariables)
                    MText_Editor_Methods.ItalicHorizontalField(material, "Material", variableName + "Material", FieldSize.small);

                TextStyles();

                CombineMesh();

                EditorGUI.indentLevel = 3;
                DontCombineInEditorEither();

                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// For perfromance, split the layout settings to two methods.
        /// If it's folded, no need to do extra calculation
        /// </summary>
        private void LayoutSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            showLayoutSettingsInEditor.target = EditorGUILayout.Foldout(showLayoutSettingsInEditor.target, new GUIContent("Layout", "Layouts are driven by Layout Groups. Although grid layout groups is the default one, it can work with any layout group. Experiment with different ones to see what you like."), true, foldOutStyle);
            Documentation(layoutDocumentationURL, "Layout Groups");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private void LayoutSettings(out Alignment anchor, out Vector3 spacing, out float width, out float height)
        {
            GridLayoutGroup gridLayout = myTarget.gameObject.GetComponent<GridLayoutGroup>();
            if (!gridLayout)
            {
                anchor = Alignment.MiddleCenter;
                spacing = Vector3.zero;
                width = 0;
                height = 0;
            }
            else
            {
                anchor = gridLayout.Anchor;
                spacing = gridLayout.Spacing;
                width = gridLayout.Width;
                height = gridLayout.Height;
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            showLayoutSettingsInEditor.target = EditorGUILayout.Foldout(showLayoutSettingsInEditor.target, new GUIContent("Layout", "Layouts are driven by Layout Groups. Although grid layout groups is the default one, it can work with any layout group. Experiment with different ones."), true, foldOutStyle);
            Documentation(layoutDocumentationURL, "Layout Groups");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showLayoutSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 0;
                GUILayout.Space(5);
                ChooseLayoutGroups();

                if (myTarget.gameObject.GetComponent<GridLayoutGroup>())
                {
                    EditorGUILayout.LabelField("Modify the attached grid layout component to modify the Layout of the text.", defaultMultilineLabel);
                }
                else if (myTarget.gameObject.GetComponent<LayoutGroup>())
                {
                    EditorGUILayout.HelpBox("Modify the attached layout component to modify the Layout of the text.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("No layout group seems to be attached to the text. If this is intentional, ignore this message. Otherwise, please add any layout group to this object. Grid Layout group is the default one.", MessageType.Warning);
                    if (GUILayout.Button("Add grid layout Group"))
                    {
                        AddGridLayout();
                    }
                }
                GUILayout.Space(5);
                //EditorGUILayout.PropertyField(_wordSpacing, new GUIContent("Word Spacing", variableName + "WordSpacing"));
                MText_Editor_Methods.ItalicHorizontalField(_wordSpacing, "Word Spacing", variableName + "WordSpacing", FieldSize.large);
                MText_Editor_Methods.ItalicHorizontalField(autoLetterSize, "Auto Letter Size", "If turned on, instead of using the predetermined size of each letter, their size is taken from the size they take in the render view.\nPlease remember, this is letter size, this doesn't modify the font size." + variableName + "AutoLetterSize", FieldSize.large);
                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void ModuleSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(useModules, GUIContent.none, GUILayout.MaxWidth(25));
            showModuleSettingsInEditor.target = EditorGUILayout.Foldout(showModuleSettingsInEditor.target, new GUIContent("Modules", modulesToolTip), true, foldOutStyle);
            Documentation("https://ferdowsur.gitbook.io/modules/", "Modules");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showModuleSettingsInEditor.faded))
            {
                Color contentDefaultColor = GUI.contentColor;

                if (myTarget.combineMeshDuringRuntime)
                    EditorGUILayout.HelpBox("Combine mesh in playmode/build is turned on, modules won't work.", MessageType.Error);

                if (!myTarget.useModules || myTarget.combineMeshDuringRuntime)
                    GUI.contentColor = toggledOffColor;

                GUILayout.Space(5);

                RunModulesSettings();

                EditorGUI.indentLevel = 2;
                GUILayout.Space(5);
                ModuleDrawer.BaseModuleContainerList("Adding", addingtoolTip + variableName + "addingModules", myTarget.addingModules, addingModules, soTarget);

                GUILayout.Space(10);
                DeleteAfterDuration();
                ModuleDrawer.BaseModuleContainerList("Deleting", deleteingtoolTip + variableName + "deletingModules", myTarget.deletingModules, deletingModules, soTarget);

                GUI.contentColor = contentDefaultColor;
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void AdvancedSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showAdvancedSettingsInEditor.target = EditorGUILayout.Foldout(showAdvancedSettingsInEditor.target, "Advanced Settings", true, foldOutStyle);
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showAdvancedSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 1;

                EditorGUILayout.Space(5);

                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginVertical(EditorStyles.toolbar);
                showAdvancedInspectorSettingsInEditor.target = EditorGUILayout.Foldout(showAdvancedInspectorSettingsInEditor.target, "Inspector Settings", true, foldOutStyle);
                GUILayout.EndVertical();

                if (EditorGUILayout.BeginFadeGroup(showAdvancedInspectorSettingsInEditor.faded))
                {
                    EditorGUI.indentLevel = 0;
                    MText_Editor_Methods.ItalicHorizontalField(hideOverwrittenVariablesFromInspector, "Hide overwritten values", "Texts under button/list sometimes have styles overwritten. This hides these variables", FieldSize.gigantic);

                    HideLetterInHierarchy();
                    EditorGUILayout.Space(5);
                    PrefabAdvancedSettings(); //mesh save setting
                    CombineMeshSettings();
                }
                EditorGUILayout.EndFadeGroup();

                GUILayout.EndVertical();

                EditorGUILayout.Space(5);

                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginVertical(EditorStyles.toolbar);
                showAdvancedBehaviorSettingsInEditor.target = EditorGUILayout.Foldout(showAdvancedBehaviorSettingsInEditor.target, "Behavior Settings", true, foldOutStyle);
                GUILayout.EndVertical();

                if (EditorGUILayout.BeginFadeGroup(showAdvancedBehaviorSettingsInEditor.faded))
                {
                    EditorGUI.indentLevel = 0;
                    MText_Editor_Methods.ItalicHorizontalField(meshPostProcess, "UV Remapping", "Project UV is the default." + variableName + "meshPostProcess, type enum", FieldSize.normal);

                    string verticiesCountTooltip = "Variable name: useIncreasedVerticiesCountForCombinedMesh, type bool \n\nApplies only to combined meshes. \nChanges mesh index format from 16 to 32 when set to true. index format 16 bit takes less memory and bandwidth. With lower capacity, if max verticies count(65536) is reached, the mesh is split into separate objects.\nDoesn't auto change the index format if it is not needed";

                    MText_Editor_Methods.ItalicHorizontalField(useIncreasedVerticiesCountForCombinedMesh, "Don't split large combined meshes", verticiesCountTooltip, FieldSize.gigantic);
                    EditorGUILayout.Space(5);

                    MText_Editor_Methods.ItalicHorizontalField(repositionOldCharacters, "Reposition old Chars", "If old text = '123' and updated new text = '1234',\nthe '123' will be moved to their correct position when entering the '4'", FieldSize.gigantic);
                    MText_Editor_Methods.ItalicHorizontalField(reApplyModulesToOldCharacters, "Re-apply modules", "If old text = old and updated new text = oldy,\ninstead of applying module to only 'y', it will apply to all chars", FieldSize.gigantic);
                    MText_Editor_Methods.ItalicHorizontalField(updateTextOncePerFrame, "Update once per frame", "If the gameobject is active in hierarchy, uses coroutine to make sure the text is only updated visually once per frame instead of wasting resources if updated multiple times by a script. This is only used if the game object is active in hierarchy and it updates at the end of frame.", FieldSize.gigantic);

                    EditorGUILayout.Space(5);

                    EditorGUILayout.LabelField(new GUIContent("Run module routine on character", "The adding module uses MonoBehavior attached to the char to run the coroutine. This way, if the text is deactivated, the module isn't interrupted."), defaultLabel);
                    EditorGUI.indentLevel = 1;
                    MText_Editor_Methods.ItalicHorizontalField(startAddingModuleFromChar, "Adding module", "If true, the adding module uses MonoBehavior attached to the char to run the coroutine. This way, if the text is deactivated, the module isn't interrupted.", FieldSize.extraLarge);
                    MText_Editor_Methods.ItalicHorizontalField(startDeletingModuleFromChar, "Deleting module", "If true, the deleting module uses MonoBehavior attached to the char to run the coroutine. This way, if the text is deactivated, the module isn't interrupted.", FieldSize.extraLarge);
                    EditorGUILayout.Space(10);
                    EditorGUI.indentLevel = 0;
                    MText_Editor_Methods.ItalicHorizontalField(destroyChildObjectsWithGameObject, "Destroy Letter With this", "If you delete the gameobject, the letters are auto deleted also even if they aren't child object.", FieldSize.gigantic);
                }
                EditorGUILayout.EndFadeGroup();

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void DebugView()
        {
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.BeginVertical(EditorStyles.toolbar);
                showDebugSettingsInEditor.target = EditorGUILayout.Foldout(showDebugSettingsInEditor.target, "Debug", true, foldOutStyle);
                GUILayout.EndVertical();
            }
            if (EditorGUILayout.BeginFadeGroup(showDebugSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 2;
                DebugLogs();
                EditorGUI.indentLevel = 2;

                GUILayout.Space(10);

                EditorGUILayout.PropertyField(wordArray);
                EditorGUILayout.PropertyField(generatedMeshes);
            }

            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
            GUILayout.Space(15);
        }

        private void DebugLogs()
        {
            MText_Editor_Methods.ItalicHorizontalField(debugLogs, "Debug Logs", "Editor only private variable." + variableName + "debugLogs", FieldSize.gigantic);

            if (!myTarget.debugLogs) return;

            GUILayout.Space(5);

            EditorGUI.indentLevel = 3;
            MText_Editor_Methods.ItalicHorizontalField(runTimeLogging, "Runtime Logs", "Log when application is playing/", FieldSize.gigantic);
            MText_Editor_Methods.ItalicHorizontalField(editorTimeLogging, "Editor Logs", "Log during editor mode", FieldSize.gigantic);

            EditorGUI.indentLevel = 4;
            GUILayout.Space(5);
            if (!myTarget.editorTimeLogging && !myTarget.runTimeLogging) return;

            MText_Editor_Methods.ItalicHorizontalField(logTextUpdates, "Text Updates", "", FieldSize.gigantic);
            MText_Editor_Methods.ItalicHorizontalField(logFontUpdates, "Font Updates", "", FieldSize.gigantic);
            MText_Editor_Methods.ItalicHorizontalField(logMaterialUpdates, "Material Updates", "", FieldSize.gigantic);
            MText_Editor_Methods.ItalicHorizontalField(logDeletedCharacters, "Deleted characters", "", FieldSize.gigantic);
            MText_Editor_Methods.ItalicHorizontalField(logSingleMeshStatus, "Single mesh status", "", FieldSize.gigantic);
        }

        private void WarningCheck()
        {
            EditorGUI.indentLevel = 0;
            if (!myTarget.Font)
                EditorGUILayout.HelpBox("No font selected", MessageType.Error);

            //if (myTarget.DoesStyleInheritFromAParent())
            if (inheritsStyleFromParents)
            {
                EditorGUILayout.HelpBox("Some values are overwritten by parent button/list.", MessageType.Info);
                GUILayout.Space(5);
            }
            else
            {
                if (!myTarget.Material)
                    EditorGUILayout.HelpBox("No material selected", MessageType.Error);
                else if (myTarget.GetComponent<Renderer>() != null)
                {
                    if (myTarget.GetComponent<Renderer>().sharedMaterial != myTarget.Material)
                    {
                        EditorGUILayout.HelpBox("Did you change the material of the mesh rendere directly? Please apply material on the 3D Text component to avoid unexpected behavior.", MessageType.Info);
                    }
                }
            }
        }

        #endregion Primary Sections

        #region Functions for main settings

        /// <summary>
        /// Direction, capitalize etc.
        /// </summary>
        private void TextStyles()
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Font Style", defaultLabel, GUILayout.MaxWidth(75));

            if (myTarget.LowerCase)
                GUI.backgroundColor = toggledOnButtonColor;
            else
                GUI.backgroundColor = toggledOffButtonColor;

            if (LeftButton(smallCase))
            {
                Undo.RecordObject(target, "Update text");
                myTarget.LowerCase = !myTarget.LowerCase;
                myTarget.Capitalize = false;
                myTarget.UpdateText();
                EditorUtility.SetDirty(myTarget);
            }

            if (myTarget.Capitalize)
                GUI.backgroundColor = toggledOnButtonColor;
            else
                GUI.backgroundColor = toggledOffButtonColor;

            if (RightButton(capitalize))
            {
                Undo.RecordObject(target, "Update text");
                myTarget.Capitalize = !myTarget.Capitalize;
                myTarget.LowerCase = false;
                myTarget.UpdateText();
                EditorUtility.SetDirty(myTarget);
            }

            GUI.backgroundColor = originalBackgroundColor;
            EditorGUILayout.EndHorizontal();
        }

        private void CombineMesh()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(combineLabel, defaultLabel, GUILayout.MinWidth(70), GUILayout.MaxWidth(80));
            MText_Editor_Methods.ItalicHorizontalField(combineMeshInEditor, "In Editor", "Combines in the Editor.\n\nbool name: 'combineMeshInEditor'", FieldSize.tiny, true);
            MText_Editor_Methods.ItalicHorizontalField(combineMeshDuringRuntime, "Runtime", "Combines during runtime. \nPlease note that enabling this might cause problems with some modules. \n\nbool name: 'combineMeshDuringRuntime'", FieldSize.tiny, true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        #endregion Functions for main settings

        #region Functions for Layout settings

        private void ApplyGridLayoutSettings(Alignment anchor, Vector2 spacing, float width, float height)
        {
            GridLayoutGroup gridLayoutGroup = myTarget.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
                return;

            if (anchor != gridLayoutGroup.Anchor)
                gridLayoutGroup.Anchor = anchor;

            if (spacing != gridLayoutGroup.Spacing)
                gridLayoutGroup.Spacing = spacing;

            if (width != gridLayoutGroup.Width)
                gridLayoutGroup.Width = width;
            if (height != gridLayoutGroup.Height)
                gridLayoutGroup.Height = height;
        }

        private void ChooseLayoutGroups()
        {
            Color defaultColor = GUI.color;

            EditorGUILayout.BeginHorizontal();
            var groups = myTarget.GetListOfAllLayoutGroups();
            for (int i = 0; i < groups.Count; i++)
            {
                if (i == 0) //First layout
                {
                    if (myTarget.gameObject.GetComponent(groups[i]))
                        GUI.color = toggledOnButtonColor;
                    else
                        GUI.color = toggledOffButtonColor;

                    if (LeftButton(new GUIContent(FormatClassName(groups[i].Name))))
                    {
                        AddLayoutComponent(groups, i);
                    }
                }
                else if (i + 1 == groups.Count) //Last layout
                {
                    if (myTarget.gameObject.GetComponent(groups[i]))
                        GUI.color = toggledOnButtonColor;
                    else
                        GUI.color = toggledOffButtonColor;

                    if (RightButton(new GUIContent(FormatClassName(groups[i].Name))))
                    {
                        AddLayoutComponent(groups, i);
                    }
                }
                else
                {
                    if (myTarget.gameObject.GetComponent(groups[i]))
                        GUI.color = toggledOnButtonColor;
                    else
                        GUI.color = toggledOffButtonColor;

                    if (MidButton(new GUIContent(FormatClassName(groups[i].Name))))
                    {
                        AddLayoutComponent(groups, i);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = defaultColor;
        }

        private void AddLayoutComponent(List<System.Type> groups, int i)
        {
            if (myTarget.GetComponent(groups[i]))
                return;

            string title = "Change Layout Group to " + FormatClassName(groups[i].Name);
            string mainDialogue = "This will overwrite all current layout settings";

            bool agreed = EditorUtility.DisplayDialog(title, mainDialogue, "Continue", "Cancel");
            if (agreed)
            {
                if (myTarget.gameObject.GetComponent<LayoutGroup>())
                    Undo.DestroyObjectImmediate(myTarget.gameObject.GetComponent<LayoutGroup>());
                Undo.AddComponent(myTarget.gameObject, groups[i]);
                //DestroyImmediate(myTarget.gameObject.GetComponent<LayoutGroup>());
                //myTarget.gameObject.AddComponent(groups[i]);

                EditorApplication.delayCall += () => UpdateGridLayoutVariables(); //this is because properties aren't having default values for some reason

                EditorApplication.delayCall += () => myTarget.gameObject.GetComponent<Modular3DText>().CleanUpdateText();
            }
        }

        private void AddGridLayout()
        {
            myTarget.gameObject.AddComponent<GridLayoutGroup>();
            EditorApplication.delayCall += () => UpdateGridLayoutVariables();
        }

        private void UpdateGridLayoutVariables()
        {
            if (myTarget.GetComponent<GridLayoutGroup>())
            {
                myTarget.GetComponent<GridLayoutGroup>().Width = 5;
                myTarget.GetComponent<GridLayoutGroup>().Height = 3;
            }
        }

        private string FormatClassName(string name)
        {
            if (name.Contains("LayoutGroup"))
                name = name.Replace("LayoutGroup", "");
            return name;
        }

        #endregion Functions for Layout settings

        #region Functions for Module settings

        private void RunModulesSettings()
        {
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginVertical(EditorStyles.toolbar);
            showModuleRunSettingsInEditor.target = EditorGUILayout.Foldout(showModuleRunSettingsInEditor.target, "Run modules on", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showModuleRunSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 2;
                MText_Editor_Methods.ItalicHorizontalField(applyModuleOnNewCharacter, "New character", "Modules are called when new characters are added." + variableName + "applyModuleOnNewCharacter", FieldSize.gigantic, true);
                MText_Editor_Methods.ItalicHorizontalField(applyModulesOnStart, "on Start", "On Start(), modules are called on all existing characters. Example: Instantiating a prefab will call the modules when it is active the first time during Unity's Start(). Note that if on enable is active, on start is ignored to avoid updating the text twice in the same frame." + variableName + "applyModulesOnStart", FieldSize.gigantic, true);
                MText_Editor_Methods.ItalicHorizontalField(applyModulesOnEnable, "on Enable", "On OnEnable(), modules are called on all existing characters. Example: Repeatedly enabling-disabling a game object will call modules when they are enabled." + variableName + "applyModulesOnEnable", FieldSize.gigantic, true);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void DeleteAfterDuration()
        {
            string toolTip = "When a character is removed, how long it takes the mesh to be deleted.\nIf set to false, when a character is deleted, it is removed instantly or after the highest duration retrievable from modules, if there is any. \nIgnored if modules are disabled.";

            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.helpBox);

            MText_Editor_Methods.ItalicHorizontalField(customDeleteAfterDuration, "Custom delete duration", toolTip + variableName + "customDeleteAfterDuration", FieldSize.extraLarge);
            if (!myTarget.customDeleteAfterDuration)
            {
                float duration = myTarget.GetDeleteDurationFromEffects();

                if (duration > 0)
                {
                    GUIContent content = new GUIContent("Delete chars after : " + duration + " seconds", toolTip + variableName + "deleteAfter");
                    EditorGUILayout.LabelField(content, defaultLabel);
                }
                else
                {
                    if (myTarget.deletingModules.Count == 0)
                    {
                        EditorGUILayout.LabelField("Letters are instantly removed after removed from text.", defaultMultilineLabel);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(new GUIContent("Letters are instantly removed after removed from text. If this is intentional, ignore this message, otherwise, please specify a custom delete duration."), defaultMultilineLabel);
                    }
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                MText_Editor_Methods.ItalicHorizontalField(deleteAfter, "Delete After", toolTip + variableName + "deleteAfter", FieldSize.small);
                GUIContent content = new GUIContent(" seconds", toolTip + variableName + "deleteAfter");
                EditorGUILayout.LabelField(content, defaultLabel);
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }

        #endregion Functions for Module settings

        #region Functions for advanced settings

        private void PrefabAdvancedSettings()
        {
            if (ShouldShowPrefabSettings())
            {
                if (PrefabUtility.IsOutermostPrefabInstanceRoot(myTarget.gameObject))
                {
                    MText_Editor_Methods.HorizontalField(canBreakOutermostPrefab, "Break outermost Prefab", "If the text isn't a child object of the prefab, it can break prefab and save the reference.", FieldSize.extraLarge);
                }
            }
            else
                MeshSaveSettings();
            EditorGUI.indentLevel = 1;

            PrefabMeshSaveSettings();
        }

        private void MeshSaveSettings()
        {
            if (myTarget.gameObject.GetComponent<MeshFilter>())
            {
                EditorGUI.indentLevel = 0;

                GUILayout.BeginVertical(EditorStyles.helpBox);

                MText_Editor_Methods.ItalicHorizontalField(autoSaveMesh, "Auto save mesh", "");
                //EditorGUILayout.PropertyField(autoSaveMesh);
                GUILayout.BeginHorizontal();

                if (!myTarget.autoSaveMesh)
                {
                    if (GUILayout.Button(new GUIContent("Save")))
                    {
                        if (myTarget.autoSaveMesh && myTarget.meshPaths.Count == 0)
                            myTarget.SaveMeshAsAsset(true);
                        else
                            myTarget.SaveMeshAsAsset(false);
                    }
                }
                if (myTarget.meshPaths.Count != 0
                    && GUILayout.Button(new GUIContent("Save as", "Save a new copy of the mesh in project")))
                {
                    myTarget.SaveMeshAsAsset(true);
                }

                GUILayout.EndHorizontal();
                if (myTarget.meshPaths.Count != 0)
                    if (myTarget.meshPaths[0].Length > 0)
                        EditorGUILayout.LabelField("Mesh path: " + myTarget.meshPaths[0]);

                if (myTarget.autoSaveMesh && myTarget.meshPaths.Count == 0)
                    EditorGUILayout.HelpBox("Auto save is turned on but no save path is detected. Please click 'Save as' and select it.", MessageType.Info);

                GUILayout.EndVertical();
            }
        }

        private void PrefabMeshSaveSettings()
        {
            if (myTarget.assetPath != "" && myTarget.assetPath != null && !EditorApplication.isPlaying)
            {
                EditorGUILayout.LabelField(myTarget.assetPath, EditorStyles.boldLabel);
                if (GUILayout.Button("Apply to prefab"))
                {
                    myTarget.ReconnectPrefabs();
                }
            }

            if ((myTarget.assetPath != "" && myTarget.assetPath != null && !EditorApplication.isPlaying))
            {
                if (GUILayout.Button("Remove prefab connection"))
                {
                    myTarget.assetPath = "";
                }
            }
            if (ShouldShowPrefabSettings())
            {
                MeshSaveSettings();
            }
        }

        private void CombineMeshSettings()
        {
            DontCombineInEditorEither();

            if (myTarget.gameObject.GetComponent<MeshFilter>())
            {
                if (GUILayout.Button(new GUIContent("Optimize mesh", "This causes the geometry and vertices of the combined mesh to be reordered internally in an attempt to improve vertex cache utilisation on the graphics hardware and thus rendering performance. This operation can take a few seconds or more for complex meshes.")))
                {
                    StaticMethods.OptimizeMesh(myTarget.gameObject.GetComponent<MeshFilter>().sharedMesh);
                }
            }
        }

        private void DontCombineInEditorEither()
        {
            if (!myTarget.combineMeshInEditor && ShouldShowPrefabSettings())
            {
                GUILayout.BeginHorizontal();

                string tooltip = "Prefabs don't allow child objects that are part of the prefab to be deleted in Editor.\n" +
                    "If you add child objects, then apply, which adds these child objects to the prefab,\n" +
                    "When changing text again, this script can't delete the old gameobjects. Just disables them. Remember to clean them up manually if you enable this.";
                EditorGUILayout.LabelField(new GUIContent("Single in Prefab", tooltip), GUILayout.Width(140));
                EditorGUILayout.PropertyField(singleInPrefab, GUIContent.none);

                GUILayout.EndHorizontal();
            }
        }

        private bool ShouldShowPrefabSettings() => PrefabUtility.IsPartOfPrefabInstance(myTarget.gameObject) || UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;

        private void HideLetterInHierarchy()
        {
            EditorGUI.indentLevel = 0;
            MText_Editor_Methods.ItalicHorizontalField(hideLettersInHierarchyInPlayMode, "Hide letters in Hierarchy", "Hide letters in Hierarchy in playmode hides the game object of letters in the hierarchy. They are still there, accessible by script, just not visible. No impact except for cleaner hierarchy.", FieldSize.gigantic);

            //GUILayout.BeginHorizontal();
            //GUIContent hideLetters = new GUIContent("Hide letters in Hierarchy in playmode", "Hides the game object of letters in the hierarchy. They are still there, accessible by script, just not visible. No impact except for cleaner hierarchy.");
            //EditorGUILayout.LabelField(hideLetters, defaultMultilineLabel);
            //EditorGUILayout.PropertyField(hideLettersInHierarchyInPlayMode, GUIContent.none, GUILayout.MaxWidth(20));
            ////MText_Editor_Methods.HorizontalField(hideLettersInHierarchyInPlayMode, "in PlayMode", "", FieldSize.large);
            //GUILayout.EndHorizontal();
        }

        #endregion Functions for advanced settings

        #region Style

        private void GenerateStyle()
        {
            textInputStyle ??= new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                padding = new RectOffset(5, 5, 5, 5),
                fontSize = 12
            };

            if (toggleStyle == null)
            {
                toggleStyle = new GUIStyle(GUI.skin.button);
                toggleStyle.margin = new RectOffset(0, 0, toggleStyle.margin.top, toggleStyle.margin.bottom);
                toggleStyle.fontStyle = FontStyle.Bold;
                toggleStyle.active.textColor = Color.yellow;
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
                    fontSize = 12
                };

                defaultLabel.normal.textColor = ThemeBasedLabelColor();
            }
            if (defaultMultilineLabel == null)
            {
                defaultMultilineLabel = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter,
                };
                defaultLabel.normal.textColor = ThemeBasedLabelColor();
            }

            //EditorStyles.popup.fontSize = 11; //commented out because couldn't find any use case
            //EditorStyles.popup.fixedHeight = 18;
        }

        private Color ThemeBasedLabelColor()
        {
            if (EditorGUIUtility.isProSkin)
                return new Color(0.9f, 0.9f, 0.9f, 0.75f);
            else
                return new Color(0.1f, 0.1f, 0.1f, 0.75f);
        }

        private bool LeftButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);

            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(0, 0, rect.width + toggleStyle.border.right, rect.height), content, toggleStyle))
                clicked = true;

            GUI.EndGroup();
            return clicked;
        }

        private bool MidButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);

            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left + toggleStyle.border.right, rect.height), content, toggleStyle))
                //if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left + toggleStyle.border.right, rect.height), content, toggleStyle))
                clicked = true;
            GUI.EndGroup();
            return clicked;
        }

        private bool RightButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);

            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left, rect.height), content, toggleStyle))
                clicked = true;
            GUI.EndGroup();
            return clicked;
        }

        #endregion Style

        #region Functions

        private void Documentation(string URL, string subject)
        {
            GUIContent doc = new GUIContent(documentationIcon, subject + " documentation\n\nURL: " + URL);
            if (GUILayout.Button(doc, EditorStyles.toolbarButton, GUILayout.Height(iconSize), GUILayout.Width(iconSize)))
            {
                Application.OpenURL(URL);
            }
        }

        /// <summary>
        /// Called on Enable
        /// </summary>
        private void FindProperties()
        {
            //text = soTarget.FindProperty("_text");

            autoSaveMesh = soTarget.FindProperty("autoSaveMesh");

            //main settings
            font = soTarget.FindProperty("_font");
            material = soTarget.FindProperty("_material");
            fontSize = soTarget.FindProperty("_fontSize");

            autoLetterSize = soTarget.FindProperty("_autoLetterSize");
            _wordSpacing = soTarget.FindProperty("_wordSpacing");

            //effects
            useModules = soTarget.FindProperty("useModules");
            startAddingModuleFromChar = soTarget.FindProperty("startAddingModuleFromChar");
            addingModules = soTarget.FindProperty("addingModules");
            startDeletingModuleFromChar = soTarget.FindProperty("startDeletingModuleFromChar");
            deletingModules = soTarget.FindProperty("deletingModules");
            customDeleteAfterDuration = soTarget.FindProperty("customDeleteAfterDuration");
            deleteAfter = soTarget.FindProperty("deleteAfter");
            applyModuleOnNewCharacter = soTarget.FindProperty("applyModuleOnNewCharacter");
            applyModulesOnStart = soTarget.FindProperty("applyModulesOnStart");
            applyModulesOnEnable = soTarget.FindProperty("applyModulesOnEnable");

            //advanced
            destroyChildObjectsWithGameObject = soTarget.FindProperty("destroyChildObjectsWithGameObject");
            repositionOldCharacters = soTarget.FindProperty("repositionOldCharacters");
            reApplyModulesToOldCharacters = soTarget.FindProperty("reApplyModulesToOldCharacters");
            //activateChildObjects = soTarget.FindProperty("activateChildObjects");

            hideOverwrittenVariablesFromInspector = soTarget.FindProperty("hideOverwrittenVariablesFromInspector");
            combineMeshInEditor = soTarget.FindProperty("combineMeshInEditor");
            singleInPrefab = soTarget.FindProperty("singleInPrefab");
            combineMeshDuringRuntime = soTarget.FindProperty("combineMeshDuringRuntime");
            hideLettersInHierarchyInPlayMode = soTarget.FindProperty("hideLettersInHierarchyInPlayMode");
            //hideLettersInHierarchyInEditMode = soTarget.FindProperty("hideLettersInHierarchyInEditMode");
            updateTextOncePerFrame = soTarget.FindProperty("updateTextOncePerFrame");

            canBreakOutermostPrefab = soTarget.FindProperty("canBreakOutermostPrefab");
            //saveObjectInScene = soTarget.FindProperty("saveObjectInScene");

            debugLogs = soTarget.FindProperty("debugLogs");
            runTimeLogging = soTarget.FindProperty("runTimeLogging");
            editorTimeLogging = soTarget.FindProperty("editorTimeLogging");
            logTextUpdates = soTarget.FindProperty("logTextUpdates");
            logFontUpdates = soTarget.FindProperty("logFontUpdates");
            logMaterialUpdates = soTarget.FindProperty("logMaterialUpdates");
            logDeletedCharacters = soTarget.FindProperty("logDeletedCharacters");
            logSingleMeshStatus = soTarget.FindProperty("logSingleMeshStatus");

            generatedMeshes = soTarget.FindProperty("generatedMeshes");

            wordArray = soTarget.FindProperty("wordArray");

            meshPostProcess = soTarget.FindProperty("meshPostProcess");
            useIncreasedVerticiesCountForCombinedMesh = soTarget.FindProperty("useIncreasedVerticiesCountForCombinedMesh");
        }

        private void LoadFoldoutValues()
        {
            showMainSettingsInEditor = new AnimBool(true);
            showMainSettingsInEditor.valueChanged.AddListener(Repaint);

            showLayoutSettingsInEditor = new AnimBool(false);
            showLayoutSettingsInEditor.valueChanged.AddListener(Repaint);

            showModuleSettingsInEditor = new AnimBool(false);
            showModuleSettingsInEditor.valueChanged.AddListener(Repaint);

            showModuleRunSettingsInEditor = new AnimBool(false);
            showModuleRunSettingsInEditor.valueChanged.AddListener(Repaint);

            showAdvancedSettingsInEditor = new AnimBool(false);
            showAdvancedSettingsInEditor.valueChanged.AddListener(Repaint);

            showAdvancedInspectorSettingsInEditor = new AnimBool(false);
            showAdvancedInspectorSettingsInEditor.valueChanged.AddListener(Repaint);

            showAdvancedBehaviorSettingsInEditor = new AnimBool(false);
            showAdvancedBehaviorSettingsInEditor.valueChanged.AddListener(Repaint);

            showDebugSettingsInEditor = new AnimBool(false);
            showDebugSettingsInEditor.valueChanged.AddListener(Repaint);
        }

        #endregion Functions
    }
}