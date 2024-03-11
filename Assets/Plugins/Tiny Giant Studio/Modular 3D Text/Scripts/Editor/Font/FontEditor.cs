/// Created by Ferdowsur Asif @ Tiny Giant Studio
using System.IO;
using TinyGiantStudio.EditorHelpers;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    /// <summary>
    /// TODO: Loop through all characters and check if any is empty
    /// </summary>

    [CustomEditor(typeof(Font))]
    public class FontEditor : Editor
    {
        private AssetSettings settings;

        private Font myTarget;
        private SerializedObject soTarget;

        private SerializedProperty modelSource;
        private SerializedProperty characters;
        private SerializedProperty showCharacterDetailsEditor;

        private SerializedProperty monoSpaceFont;
        private SerializedProperty monoSpaceSpacing;
        private SerializedProperty unitPerEM;
        private SerializedProperty useUpperCaseLettersIfLowerCaseIsMissing;
        private SerializedProperty spaceSpacing; //to-do: rename to word spacing?
        private SerializedProperty characterSpacing;
        private SerializedProperty lineHeight;

        private SerializedProperty fallbackFonts;

        private SerializedProperty enableKerning;
        private SerializedProperty kerningMultiplier;
        private SerializedProperty kernTable;

        private GUIStyle foldOutStyle = null;
        private GUIStyle defaultLabel = null;
        private GUIStyle searchField = null;
        private GUIStyle defaultMultilineLabel = null;

        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);
        private static Color unimportedLabelColor = new Color(239 / 255f, 239 / 255f, 239 / 255f, 0.9f);

        private AnimBool showSettings;
        private AnimBool showFallBackSettingsInEditor;
        private AnimBool showSpacingSettingsInEditor;
        private AnimBool showBaseSettingsInEditor;
        private AnimBool showKerningTableInEditor;
        private AnimBool showCharacterListInEditor;

        private Texture rightIcon;
        private Texture leftIcon;
        private Texture deleteIcon;
        private Texture addIcon;

        private readonly int characterCountInAPage = 10;
        private int currentPage;

        private readonly int kerningCountInAPage = 20;
        private int kerningCurrentPage;

        private FontInstanceUpdater _myFontInstanceUpdater;

        private FontInstanceUpdater MyFontInstanceUpdater
        {
            get
            {
                if (_myFontInstanceUpdater == null)
                    _myFontInstanceUpdater = new FontInstanceUpdater();
                return _myFontInstanceUpdater;
            }
        }

        private void OnEnable()
        {
            myTarget = (Font)target;
            soTarget = new SerializedObject(target);

            rightIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Layouts/Utility/Editor Icons/Icon_Right.png") as Texture;
            leftIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Layouts/Utility/Editor Icons/Icon_Left.png") as Texture;

            addIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Plus.png") as Texture;
            deleteIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/Icon_Cross.png") as Texture;

            UpdateAnimBoolsForFoldout();

            modelSource = soTarget.FindProperty("modelSource");
            useUpperCaseLettersIfLowerCaseIsMissing = soTarget.FindProperty("useUpperCaseLettersIfLowerCaseIsMissing");

            unitPerEM = soTarget.FindProperty("unitPerEM");
            monoSpaceFont = soTarget.FindProperty("monoSpaceFont");
            monoSpaceSpacing = soTarget.FindProperty("monoSpaceSpacing");
            spaceSpacing = soTarget.FindProperty("emptySpaceSpacing");
            characterSpacing = soTarget.FindProperty("characterSpacing");
            lineHeight = soTarget.FindProperty("lineHeight");

            characters = soTarget.FindProperty("characters");
            showCharacterDetailsEditor = soTarget.FindProperty("showCharacterDetailsEditor");
            fallbackFonts = soTarget.FindProperty("fallbackFonts");

            enableKerning = soTarget.FindProperty("enableKerning");
            kerningMultiplier = soTarget.FindProperty("kerningMultiplier");
            kernTable = soTarget.FindProperty("kernTable");

            if (!settings)
                settings = StaticMethods.VerifySettings(settings);
        }

        private void UpdateAnimBoolsForFoldout()
        {
            showSettings = new AnimBool(false);
            showSettings.valueChanged.AddListener(Repaint);
            showFallBackSettingsInEditor = new AnimBool(false);
            showFallBackSettingsInEditor.valueChanged.AddListener(Repaint);
            showSpacingSettingsInEditor = new AnimBool(false);
            showSpacingSettingsInEditor.valueChanged.AddListener(Repaint);
            showBaseSettingsInEditor = new AnimBool(false);
            showBaseSettingsInEditor.valueChanged.AddListener(Repaint);
            showKerningTableInEditor = new AnimBool(false);
            showKerningTableInEditor.valueChanged.AddListener(Repaint);

            showCharacterListInEditor = new AnimBool(false);
            showCharacterListInEditor.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            GenerateStyle();
            EditorGUI.BeginChangeCheck();

            WarningsCheck();

            GUILayout.Space(5);

            Settings();

            GUILayout.Space(5);
            CharacterList();
            GUILayout.Space(5);
            KerningSettings();
            GUILayout.Space(5);
            FallBackFont();

            GUILayout.Space(5);
            if (GUILayout.Button("Recreate all texts with this font in the scene", GUILayout.Height(25)))
            {
                MyFontInstanceUpdater.ApplyFontChangesToTheScene(target as Font);
            }

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                ApplyFontChanges();
            }
        }

        private void WarningsCheck()
        {
            string warning = string.Empty;
            for (int i = 0; i < myTarget.characters.Count; i++)
            {
                if (!myTarget.characters[i].meshPrefab && !myTarget.characters[i].prefab)
                    warning += "Char: " + myTarget.characters[i].character + " Doesn't have any mesh or prefab.\n";
            }
            if (warning.Length > 0) EditorGUILayout.HelpBox(warning, MessageType.Warning);
            //if (myTarget.emptySpaceSpacing > 10) EditorGUILayout.HelpBox("Base spacing seems to be set too high. If it's intended, please ignore this message.", MessageType.Warning);
            //if (myTarget.characterSpacing > 10) EditorGUILayout.HelpBox("Character spacing seems to be set too high. If it's intended, please ignore this message.", MessageType.Warning);
        }

        private void Settings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showSettings.target = EditorGUILayout.Foldout(showSettings.target, "Settings", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showSettings.faded))
            {
                EditorGUILayout.PropertyField(unitPerEM);
                float defaultWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 180;
                EditorGUILayout.PropertyField(useUpperCaseLettersIfLowerCaseIsMissing, new GUIContent("Uppercase if no lowercase"));
                //MText_Editor_Methods.HorizontalField(useUpperCaseLettersIfLowerCaseIsMissing, "Uppercase if no lowercase", "", FieldSize.extraLarge);
                EditorGUIUtility.labelWidth = defaultWidth;
                SpacingSettings();
                Data();
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        private void FallBackFont()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUIContent fallBackFoldoutTitle = new GUIContent("Fallback Fonts", "If this font has missing characters, it will try to get the character from fallback font");
            showFallBackSettingsInEditor.target = EditorGUILayout.Foldout(showFallBackSettingsInEditor.target, fallBackFoldoutTitle, true, foldOutStyle);

            GUIContent add = new GUIContent(addIcon, "Add new fallback font");
            if (GUILayout.Button(add, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                myTarget.fallbackFonts.Add(null);
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndHorizontal();

            if (EditorGUILayout.BeginFadeGroup(showFallBackSettingsInEditor.faded))
            {
                EditorGUI.indentLevel = 0;

                if (myTarget.fallbackFonts.Count == 0)
                    EditorGUILayout.LabelField("No fallback fonts have been added", defaultMultilineLabel);

                for (int i = 0; i < myTarget.fallbackFonts.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    if (fallbackFonts.arraySize > i)
                    {
                        if (myTarget.fallbackFonts[i] == myTarget)
                        {
                            Debug.LogError("Unnecessary self reference found on fallback font :" + i, myTarget);
                            myTarget.fallbackFonts.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(fallbackFonts.GetArrayElementAtIndex(i), GUIContent.none);

                            if (GUILayout.Button("-", GUILayout.MaxWidth(30)))
                            {
                                Repaint();
                                Undo.RecordObject(myTarget, myTarget.name);
                                myTarget.fallbackFonts.RemoveAt(i);
                                EditorUtility.SetDirty(myTarget);
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        private void SpacingSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showSpacingSettingsInEditor.target = EditorGUILayout.Foldout(showSpacingSettingsInEditor.target, "Spacing", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showSpacingSettingsInEditor.faded))
            {
                MText_Editor_Methods.HorizontalField(characterSpacing, "Character", "Text's character spacing = font's character spacing * text's character spacing", FieldSize.normal);
                MText_Editor_Methods.HorizontalField(spaceSpacing, "Word", "Word spacing and spacing for unavailable characters", FieldSize.normal);
                MText_Editor_Methods.HorizontalField(lineHeight, "Line Height", "", FieldSize.normal);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(monoSpaceFont, new GUIContent("is Mono Space", "Monospace means all characters are spaced equally.\nIf turned on, individual spacing value from list below is ignored. The information is not removed to avoid accidentally turning it on ruin the font. \nCharacter spacing is used for everything.\nThis is not used if auto letter size is turned on."));
                EditorGUILayout.PropertyField(monoSpaceSpacing, new GUIContent("Mono spacing"));
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        private void Data()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showBaseSettingsInEditor.target = EditorGUILayout.Foldout(showBaseSettingsInEditor.target, "Data", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showBaseSettingsInEditor.faded))
            {
                GUILayout.Space(5);
                GetFontBytes();
                GUILayout.Space(5);
                GetCharactersFromFont();
                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void GetFontBytes()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            if (myTarget.fontBytes != null)
            {
                if (myTarget.fontBytes.Length > 0)
                    EditorGUILayout.LabelField("Font bytes: " + myTarget.fontBytes.Length);
                else
                    EditorGUILayout.LabelField("No TTF font bytes found.");
            }
            else
                EditorGUILayout.LabelField("No TTF font bytes found. It's completely fine. No missing character will be generated");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select TTF font", GUILayout.MinHeight(30)))
            {
                EditorApplication.delayCall += () => SelectTTFFont();
            }
            if (GUILayout.Button("Clear TTF data", GUILayout.MinHeight(30)))
            {
                myTarget.fontBytes = null;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void SelectTTFFont()
        {
            string path = EditorUtility.OpenFilePanel("Select ttf font", "", "ttf");

            if (!string.IsNullOrEmpty(path))
            {
                myTarget.SetFontBytes((File.ReadAllBytes(path)));
                EditorUtility.SetDirty(myTarget);
            }
        }

        private void GetCharactersFromFont()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;
            EditorGUILayout.PropertyField(modelSource);

            if (GUILayout.Button("Update characters from model file", GUILayout.MinHeight(30)))
            {
                string mainDialogue = "Please note :\n" +
                    "When overwriting or adding characters added from the model file will have their data like spacing taken from the render view size.";
                if (myTarget.modelSource == null)
                    mainDialogue = "Source model file is not found.\nOverwriting in this case will just clear the existing characters.\nHow would you like to proceed";

                int option = EditorUtility.DisplayDialogComplex("Update font character list from source model",
   mainDialogue,
   "Update",
   "Overwrite old character list",
   "Cancel");

                switch (option)
                {
                    case 0:
                        Debug.Log("1");
                        myTarget.UpdateCharacterList(true);
                        EditorUtility.SetDirty(target);
                        break;
                    // overwrite.
                    case 1:
                        Debug.Log("2");
                        myTarget.UpdateCharacterList(true);
                        EditorUtility.SetDirty(target);
                        break;
                    // Dont overwrite.
                    //case 2:
                    //    Debug.Log("3");
                    //    myTarget.UpdateCharacterList(false);
                    //    EditorUtility.SetDirty(target);
                    //break;
                    default:
                        break;
                }
            }
            GUILayout.EndVertical();
        }

        #region Character Set

        private void CharacterList()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            showCharacterListInEditor.target = EditorGUILayout.Foldout(showCharacterListInEditor.target, "Characters (" + myTarget.characters.Count + ")", true, foldOutStyle);
            GUIContent add = new GUIContent(addIcon, "Add new character");
            if (GUILayout.Button(add, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                Character character = new Character();
                myTarget.characters.Add(character);
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showCharacterListInEditor.faded))
            {
                EditorGUI.indentLevel = 0;

                if (myTarget.beingSearched != null)
                {
                    int lastPage = (myTarget.characters.Count / characterCountInAPage) + 1;
                    PageNavigation(lastPage);
                }

                GUILayout.Space(5);

                if (myTarget.beingSearched == null)
                    myTarget.beingSearched = "";

                if (myTarget.beingSearched != null)
                {
                    if (myTarget.beingSearched.Length == 0)
                        AllCreateCharacterList();
                    else
                        SearchedCharacterList();
                }
                else
                {
                    AllCreateCharacterList();
                }
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        private void SearchItem()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(2), GUILayout.MaxHeight(15));
            myTarget.beingSearched = GUILayout.TextField(myTarget.beingSearched, searchField, GUILayout.MinWidth(10));
            GUILayout.EndHorizontal();
        }

        private void AllCreateCharacterList()
        {
            int startingNumber = currentPage * characterCountInAPage;
            if (startingNumber >= myTarget.characters.Count - 1)
                startingNumber = 0;
            int endingNumber = startingNumber + characterCountInAPage;
            if (endingNumber >= myTarget.characters.Count)
                endingNumber = myTarget.characters.Count;

            int lastPage = (myTarget.characters.Count / characterCountInAPage) + 1;

            //PageNavigation(lastPage);
            GUILayout.Space(5);

            EditorGUI.indentLevel = 0;
            for (int i = startingNumber; i < endingNumber; i++)
            {
                if (characters.arraySize > i)
                {
                    if (i != startingNumber)
                        GUILayout.Space(10);

                    CharacterItem(i);
                }
            }

            //if (currentPage == Mathf.FloorToInt(myTarget.characters.Count / characterCountInAPage))
            //{
            //}
            GUILayout.Space(5);

            PageNavigation(lastPage);
        }

        private void SearchedCharacterList()
        {
            for (int i = 0; i < myTarget.characters.Count; i++)
            {
                if (characters.arraySize > i)
                {
                    if (myTarget.beingSearched.Contains(myTarget.characters[i].character.ToString()))
                        CharacterItem(i);
                }
            }
        }

        private void CharacterItem(int i)
        {
            float defaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            EditorGUI.indentLevel = 0;

            GUILayout.BeginVertical(EditorStyles.helpBox);

            CharacterItemTitle(i);

            if (myTarget.showCharacterDetailsEditor)
            {
                GUILayout.BeginHorizontal();
                DrawCharacterPreview(i, 40);

                GUILayout.BeginVertical();
                RenderReferences(i, 100);

                EditorGUILayout.LabelField("Glyph Index: " + myTarget.characters[i].glyphIndex.ToString(), defaultLabel);

                GUILayout.BeginHorizontal();
                GUIContent spacingContent = new GUIContent("Spacing", "");
                EditorGUILayout.LabelField(spacingContent, defaultLabel, GUILayout.MaxWidth(50));
                EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("spacing"), GUIContent.none, GUILayout.MinWidth(40));
                GUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Left side bearing: " + myTarget.characters[i].leftSideBearing.ToString(), defaultLabel);

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                Offset(i);

                GUILayout.Space(5);
                int kernReferences = myTarget.KerningReferencesCount(i);
                EditorGUILayout.LabelField(new GUIContent("Kerning references: " + kernReferences), defaultLabel);

                if (kernReferences > 0)
                    KernTableTitle();

                for (int j = 0; j < myTarget.kernTable.Count; j++)
                {
                    bool leftModifiabled = true;
                    bool rightModifiabled = true;

                    if (myTarget.kernTable[j].left == myTarget.characters[i].character)
                        leftModifiabled = false;
                    if (myTarget.kernTable[j].right == myTarget.characters[i].character)
                        rightModifiabled = false;

                    if (!leftModifiabled || !rightModifiabled)
                        DrawKerningElement(j, leftModifiabled, rightModifiabled);
                }

            }
            else
            {
                GUILayout.BeginHorizontal();
                DrawCharacterPreview(i, 40);
                RenderReferences(i, 100);
                GUILayout.EndHorizontal();
            }

            EditorGUIUtility.labelWidth = defaultWidth;
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        private void CharacterItemTitle(int i)
        {
            if (i >= myTarget.characters.Count)
                return;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUI.contentColor = unimportedLabelColor;
            EditorGUILayout.LabelField(new GUIContent("Character", "The character this item will use."), defaultLabel, GUILayout.MaxWidth(55));

            GUI.contentColor = openedFoldoutTitleColor;
            EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("character"), GUIContent.none);

            GUI.contentColor = Color.white;

            if (GUILayout.Button(deleteIcon, defaultLabel, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
            {
                Undo.RecordObject(myTarget, myTarget.name);
                myTarget.characters.RemoveAt(i);
                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(myTarget);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawCharacterPreview(int i, int size)
        {
            if (i >= myTarget.characters.Count)
                return;

            Texture2D texture = null;

            if (myTarget.characters[i].prefab)
                texture = AssetPreview.GetAssetPreview(myTarget.characters[i].prefab);
            //texture = AssetPreview.GetAssetPreview(myTarget.characters[i].prefab);
            else if (myTarget.characters[i].meshPrefab)
                texture = AssetPreview.GetAssetPreview(myTarget.characters[i].meshPrefab);

            if (texture)
            {
                //texture = FlipTexture(texture); //Flipping takes too much resources
                GUILayout.Box(texture, GUIStyle.none, GUILayout.MaxWidth(size), GUILayout.MaxHeight(size));
            }
        }

        private void RenderReferences(int i, int size)
        {
            if (i >= myTarget.characters.Count)
                return;

            float labelWidth = 50;
            if (myTarget.characters[i].prefab)
            {
                try
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Prefab", ""), defaultLabel, GUILayout.MaxWidth(labelWidth));
                    EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("prefab"), GUIContent.none, GUILayout.MinWidth(size));
                    GUILayout.EndHorizontal();
                }
                catch
                {
                }
            }
            else if (myTarget.characters[i].meshPrefab)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Mesh", ""), defaultLabel, GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("meshPrefab"), GUIContent.none, GUILayout.MinWidth(size));
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Prefab", ""), defaultLabel, GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("prefab"), GUIContent.none, GUILayout.MinWidth(size));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Mesh", ""), defaultLabel, GUILayout.MaxWidth(labelWidth));
                EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("meshPrefab"), GUIContent.none, GUILayout.MinWidth(size));
                GUILayout.EndHorizontal();
            }
        }

        //private void Offset(int i, int size)
        private void Offset(int i)
        {
            EditorGUI.indentLevel = 0;
            int labelWidth = 11;
            int propertyWidth = 35;

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Offset"), defaultLabel, GUILayout.MaxWidth(50));

            GUILayout.Label(new GUIContent("X"), defaultLabel, GUILayout.MaxHeight(15), GUILayout.MaxWidth(labelWidth));
            EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("xOffset"), GUIContent.none, GUILayout.MaxWidth(propertyWidth));

            GUILayout.Label(new GUIContent("Y"), defaultLabel, GUILayout.MaxHeight(15), GUILayout.MaxWidth(labelWidth));
            EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("yOffset"), GUIContent.none, GUILayout.MaxWidth(propertyWidth));

            GUILayout.Label(new GUIContent("Z"), defaultLabel, GUILayout.MaxHeight(15), GUILayout.MaxWidth(labelWidth));
            EditorGUILayout.PropertyField(characters.GetArrayElementAtIndex(i).FindPropertyRelative("zOffset"), GUIContent.none, GUILayout.MaxWidth(propertyWidth));

            GUILayout.EndHorizontal();
        }

        private void PageNavigation(int lastPage)
        {
            EditorGUI.indentLevel = 0;

            GUILayout.BeginHorizontal();
            GUIContent prev = new GUIContent(leftIcon, "Previous page");
            if (GUILayout.Button(prev, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                currentPage--;
                if (currentPage < 0)
                    currentPage = myTarget.characters.Count / characterCountInAPage;
                EditorUtility.SetDirty(target);
            }
            GUILayout.BeginVertical();

            string pageNumber = "Page : " + (currentPage + 1) + " / " + lastPage;
            EditorGUILayout.LabelField(pageNumber, EditorStyles.centeredGreyMiniLabel, GUILayout.MinWidth(100));
            //GUILayout.Label("Details", GUILayout.MaxWidth(43));

            GUILayout.BeginHorizontal();
            SearchItem();
            GUILayout.Label(new GUIContent("Details"), defaultLabel, GUILayout.MaxHeight(15), GUILayout.MaxWidth(40));
            EditorGUILayout.PropertyField(showCharacterDetailsEditor, GUIContent.none, GUILayout.MaxWidth(25));

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUIContent next = new GUIContent(rightIcon, "Next page");
            if (GUILayout.Button(next, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                currentPage++;
                if (currentPage > myTarget.characters.Count / characterCountInAPage)
                    currentPage = 0;
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndHorizontal();
        }

        #endregion Character Set

        private void KerningSettings()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 0;

            string title = "Kerning Table";
            if (myTarget.kernTable.Count > 0) title += " (" + myTarget.kernTable.Count + ")";

            GUIContent kernTableFoldoutTitleContent = new GUIContent(title, "Legacy 'kern' table");
            GUILayout.BeginVertical(EditorStyles.toolbar);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(enableKerning, GUIContent.none, GUILayout.MaxWidth(26));
            showKerningTableInEditor.target = EditorGUILayout.Foldout(showKerningTableInEditor.target, kernTableFoldoutTitleContent, true, foldOutStyle);
            GUIContent add = new GUIContent(addIcon, "Add new kerning info");
            if (GUILayout.Button(add, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                myTarget.kernTable.Add(new KerningPair());
                EditorUtility.SetDirty(target);
                return;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showKerningTableInEditor.faded))
            {
                EditorGUI.indentLevel = 0;

                EditorGUILayout.PropertyField(kerningMultiplier, new GUIContent("Multiplier"));

                GUILayout.Space(5);

                //if (myTarget.kernTable.Count > 19)
                {
                    int lastPage = (myTarget.kernTable.Count / kerningCountInAPage) + 1;
                    KernPageNavigation(lastPage);

                    DrawPageKernTable();
                    KernPageNavigation(lastPage);
                }
                //else
                //{
                //    //DrawFullKernTable();
                //}
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.EndVertical();
        }

        private void DrawPageKernTable()
        {
            EditorGUI.indentLevel = 1;
            if (myTarget.kernTable == null)
                return;
            if (myTarget.kernTable.Count == 0)
            {
                EditorGUILayout.LabelField(new GUIContent("No kern table found"), defaultMultilineLabel);
                return;
            }

            int startingNumber = kerningCurrentPage * kerningCountInAPage;
            if (startingNumber >= myTarget.kernTable.Count - 1)
                startingNumber = 0;

            int endingNumber = startingNumber + kerningCountInAPage;
            if (endingNumber >= myTarget.kernTable.Count)
                endingNumber = myTarget.kernTable.Count;

            KernTableTitle();

            for (int i = startingNumber; i < endingNumber; i++)
            {
                EditorGUI.indentLevel = 0;
                DrawKerningElement(i);
            }
        }

        private void KernPageNavigation(int lastPage)
        {
            if (myTarget.kernTable.Count < kerningCountInAPage)
                return;

            EditorGUI.indentLevel = 0;

            GUILayout.BeginHorizontal();
            GUIContent prev = new GUIContent(leftIcon, "Previous page");
            if (GUILayout.Button(prev, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                kerningCurrentPage--;
                if (kerningCurrentPage < 0)
                    kerningCurrentPage = myTarget.kernTable.Count / kerningCountInAPage;
                EditorUtility.SetDirty(target);
            }
            GUILayout.BeginVertical();

            string pageNumber = "Page : " + (kerningCurrentPage + 1) + " / " + lastPage;
            EditorGUILayout.LabelField(pageNumber, EditorStyles.centeredGreyMiniLabel, GUILayout.MinWidth(100));

            GUILayout.BeginHorizontal();
            SearchItem();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUIContent next = new GUIContent(rightIcon, "Next page");
            if (GUILayout.Button(next, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                kerningCurrentPage++;
                if (kerningCurrentPage > myTarget.kernTable.Count / kerningCountInAPage)
                    kerningCurrentPage = 0;
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndHorizontal();
        }

        //void DrawFullKernTable()
        //{
        //    EditorGUI.indentLevel = 1;
        //    if (myTarget.kernTable == null)
        //        return;
        //    if (myTarget.kernTable.Count == 0)
        //    {
        //        EditorGUILayout.LabelField(new GUIContent("No kern table found"), defaultMultilineLabel);
        //        return;
        //    }

        //    int startingIndex = 0;
        //    int lastNumber = myTarget.kernTable.Count;

        //    KernTableTitle();

        //    for (int i = startingIndex; i < lastNumber; i++)
        //    {
        //        EditorGUI.indentLevel = 0;
        //        DrawKerningElement(i);
        //    }
        //}

        private void KernTableTitle()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Left", defaultMultilineLabel, GUILayout.MaxWidth(100));
            GUILayout.Label("Offset", defaultMultilineLabel);
            GUILayout.Label("Right", defaultMultilineLabel, GUILayout.MaxWidth(100));
            GUILayout.Label("", defaultMultilineLabel, GUILayout.MaxWidth(15));
            GUILayout.EndHorizontal();
        }

        private void DrawKerningElement(int i, bool leftModifiable = true, bool rightModifiable = true)
        {
            GUILayout.BeginHorizontal();
            var left = kernTable.GetArrayElementAtIndex(i).FindPropertyRelative("left");
            if (!leftModifiable)
                GUI.enabled = false;

            EditorGUILayout.PropertyField(left, GUIContent.none, GUILayout.MaxWidth(100));

            GUI.enabled = true;

            EditorGUIUtility.labelWidth = 10;
            var offset = kernTable.GetArrayElementAtIndex(i).FindPropertyRelative("offset");
            EditorGUILayout.PropertyField(offset, new GUIContent("x"));

            var right = kernTable.GetArrayElementAtIndex(i).FindPropertyRelative("right");
            if (!rightModifiable)
                GUI.enabled = false;

            EditorGUILayout.PropertyField(right, GUIContent.none, GUILayout.MaxWidth(100));
            GUI.enabled = true;

            if (GUILayout.Button(deleteIcon, defaultLabel, GUILayout.MaxWidth(15), GUILayout.MaxHeight(15)))
            {
                myTarget.kernTable.RemoveAt(i);
                EditorUtility.SetDirty(this);
            }

            GUILayout.EndHorizontal();
        }

        //string StartingChar(int index)
        //{
        //    for (int j = 0; j < myTarget.characters.Count; j++)
        //    {
        //        if (myTarget.characters[j].glyphIndex == index)
        //            return myTarget.characters[j].character.ToString();
        //    }
        //    return "Uknown";
        //}

        //Called after fields are changed in inspector
        private void ApplyFontChanges()
        {
            MyFontInstanceUpdater.ApplyFontChangesToTheScene(target as Font);
        }

        private void GenerateStyle()
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
                foldOutStyle = new GUIStyle(EditorStyles.foldout);
                foldOutStyle.overflow = new RectOffset(-10, 0, 3, 0);
                foldOutStyle.padding = new RectOffset(15, 0, -3, 0);
                foldOutStyle.fontStyle = FontStyle.Bold;
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }

            if (defaultLabel == null)
            {
                defaultLabel = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    fontStyle = FontStyle.Italic
                };
            }
            if (searchField == null)
            {
                searchField = new GUIStyle(EditorStyles.toolbarSearchField);
            }

            if (defaultMultilineLabel == null)
            {
                defaultMultilineLabel = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter,
                };
                defaultMultilineLabel.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
            }
        }
    }
}