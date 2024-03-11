using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using TinyGiantStudio.Modules;

#if MODULAR_3D_TEXT

using TinyGiantStudio.Text;

#endif

using TinyGiantStudio.EditorHelpers;

namespace TinyGiantStudio.Layout
{
    [CustomEditor(typeof(GridLayoutGroup))]
    public class GridLayoutGroupEditor : Editor
    {
#if MODULAR_3D_TEXT
        public AssetSettings settings;
#endif

        private GridLayoutGroup myTarget;
        private SerializedObject soTarget;

        private SerializedProperty autoItemSize;

        //SerializedProperty justiceHorizontal;
        private SerializedProperty justiceHorizontalPercent;

        //SerializedProperty justiceVertical;
        //SerializedProperty JusticeVerticalPercent;
        private SerializedProperty spacing;

        private SerializedProperty width;
        private SerializedProperty height;
        private SerializedProperty lines;
        private SerializedProperty bounds;
        private SerializedProperty lineSpacingStyle;
        private SerializedProperty verticalOverflow;
        private SerializedProperty elementUpdater;

        private SerializedProperty alwaysUpdateInPlayMode;
        private SerializedProperty alwaysUpdateBounds;

        //style
        private static GUIStyle toggleStyle = null;

        private static GUIStyle foldOutStyle = null;
        private static GUIStyle defaultLabel = null;

        private AnimBool showDebug;

        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);
        private static Color toggledOnButtonColor = Color.white;
        private static Color toggledOffButtonColor = Color.gray;

        private Texture justiceHorizontalTexture;

        private void OnEnable()
        {
            myTarget = (GridLayoutGroup)target;
            soTarget = new SerializedObject(target);

            FindProperties();

#if MODULAR_3D_TEXT
            if (!settings)
                settings = StaticMethods.VerifySettings(settings);
#endif

            justiceHorizontalTexture = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Layouts/Utility/Editor Icons/Justice Horizontal.png") as Texture;

            if (myTarget.gameObject.GetComponentInParent<Canvas>())
            {
                if (!myTarget.gameObject.GetComponent<RectTransform>())
                {
                    myTarget.gameObject.AddComponent<RectTransform>();
                }
            }
            else
            {
                if (myTarget.gameObject.GetComponent<RectTransform>())
                {
                    MText_Editor_Methods.RemoveRectTransform(myTarget.gameObject);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            GenerateStyle();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space(6);
            DrawSize();
            EditorGUILayout.Space(6);
            DrawAlignment();
            EditorGUILayout.Space(6);
            DrawControls();

            EditorGUILayout.Space(5);
            if (ModuleDrawer.ElementUpdatersExist())
                ModuleDrawer.ElementUpdaterContainerList("Element Updater", "", myTarget.elementUpdater, elementUpdater, soTarget);
            EditorGUILayout.Space(5);

            MText_Editor_Methods.ItalicHorizontalField(alwaysUpdateInPlayMode, "Always update in playmode", "For performance, it's better to leave it to false and call UpdateLayout() after making changes.\nTurn this on if you are in a hurry or testing stuff.", FieldSize.gigantic);
            MText_Editor_Methods.ItalicHorizontalField(alwaysUpdateBounds, "Always update bounds", "For performance, it's better to leave it to false and call GetAllChildBounds() when a bound(size of an element) changes", FieldSize.gigantic);

            EditorGUILayout.Space(15);
            DrawDebug();

            if (EditorGUI.EndChangeCheck())
            {
                Alignment anchor = myTarget.Anchor;
                if (soTarget.ApplyModifiedProperties())
                {
#if MODULAR_3D_TEXT
                    if (myTarget.GetComponent<Modular3DText>())
                    {
                        if (anchor != myTarget.Anchor)
                        {
                            myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                        }
                        else
                        {
                            //if (!myTarget.GetComponent<Modular3DText>().ShouldItCreateChild())
                            {
                                myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                            }
                        }
                    }
#endif
                }
                //EditorUtility.SetDirty(myTarget);
            }
        }

        private void DrawControls()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Spacing", defaultLabel, GUILayout.MaxWidth(75));
            EditorGUILayout.PropertyField(spacing, GUIContent.none);
            GUILayout.EndHorizontal();

#if MODULAR_3D_TEXT
            if (!myTarget.GetComponent<Modular3DText>())
#endif
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(autoItemSize);
                MText_Editor_Methods.HorizontalField(lineSpacingStyle, "Line Spacing Style", "", FieldSize.large);
            }
            MText_Editor_Methods.HorizontalField(verticalOverflow, "Vertical Overflow", "", FieldSize.large);
        }

        private void DrawAlignment()
        {
            Color originalColor = GUI.color;

            GUILayout.BeginHorizontal();

            if (IsHorizontallyLeft())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (LeftButton(EditorGUIUtility.IconContent("d_align_horizontally_left")))
            {
                Undo.RecordObject(myTarget, "Update layout");
                if (myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.UpperRight)
                    myTarget.Anchor = Alignment.UpperLeft;
                if (myTarget.Anchor == Alignment.MiddleCenter || myTarget.Anchor == Alignment.MiddleRight)
                    myTarget.Anchor = Alignment.MiddleLeft;
                if (myTarget.Anchor == Alignment.LowerCenter || myTarget.Anchor == Alignment.LowerRight)
                    myTarget.Anchor = Alignment.LowerLeft;
                EditorUtility.SetDirty(myTarget);
            }

            if (IsHorizontallyCentered())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (MidButton(EditorGUIUtility.IconContent("d_align_horizontally_center")))
            {
                Undo.RecordObject(myTarget, "Update layout");
                if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.UpperRight)
                    myTarget.Anchor = Alignment.UpperCenter;
                if (myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.MiddleRight)
                    myTarget.Anchor = Alignment.MiddleCenter;
                if (myTarget.Anchor == Alignment.LowerLeft || myTarget.Anchor == Alignment.LowerRight)
                    myTarget.Anchor = Alignment.LowerCenter;
                EditorUtility.SetDirty(myTarget);
            }

            if (IsHorizontallyRight())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (RightButton(EditorGUIUtility.IconContent("d_align_horizontally_right")))
            {
                Undo.RecordObject(myTarget, "Update layout");
                if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.UpperCenter)
                    myTarget.Anchor = Alignment.UpperRight;
                if (myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.MiddleCenter)
                    myTarget.Anchor = Alignment.MiddleRight;
                if (myTarget.Anchor == Alignment.LowerLeft || myTarget.Anchor == Alignment.LowerCenter)
                    myTarget.Anchor = Alignment.LowerRight;
                EditorUtility.SetDirty(myTarget);
            }

            GUI.color = originalColor;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (IsVerticallyUp())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (LeftButton(EditorGUIUtility.IconContent("d_align_vertically_top")))
            {
                Undo.RecordObject(myTarget, "Update layout");
                if (myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.LowerLeft)
                    myTarget.Anchor = Alignment.UpperLeft;
                if (myTarget.Anchor == Alignment.MiddleCenter || myTarget.Anchor == Alignment.LowerCenter)
                    myTarget.Anchor = Alignment.UpperCenter;
                if (myTarget.Anchor == Alignment.MiddleRight || myTarget.Anchor == Alignment.LowerRight)
                    myTarget.Anchor = Alignment.UpperRight;
                EditorUtility.SetDirty(myTarget);
            }

            if (IsVerticallyMiddle())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (MidButton(EditorGUIUtility.IconContent("d_align_vertically_center")))
            {
                Undo.RecordObject(myTarget, "Update layout");
                if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.LowerLeft)
                    myTarget.Anchor = Alignment.MiddleLeft;
                if (myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.LowerCenter)
                    myTarget.Anchor = Alignment.MiddleCenter;
                if (myTarget.Anchor == Alignment.UpperRight || myTarget.Anchor == Alignment.LowerRight)
                    myTarget.Anchor = Alignment.MiddleRight;
                EditorUtility.SetDirty(myTarget);
            }

            if (IsVerticallyBottom())
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (RightButton(EditorGUIUtility.IconContent("d_align_vertically_bottom")))
            {
                Undo.RecordObject(myTarget, "Update layout");
                if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.MiddleLeft)
                    myTarget.Anchor = Alignment.LowerLeft;
                if (myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.MiddleCenter)
                    myTarget.Anchor = Alignment.LowerCenter;
                if (myTarget.Anchor == Alignment.UpperRight || myTarget.Anchor == Alignment.MiddleRight)
                    myTarget.Anchor = Alignment.LowerRight;
                EditorUtility.SetDirty(myTarget);
            }

            GUI.color = originalColor;

            //EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(1.5f), GUILayout.MinWidth(1.5f));

            //var verticalJustice = Resources.Load("Justice Vertical") as Texture;

            //GUIContent verticalJusticeContent;

            //if (verticalJustice)
            //    verticalJusticeContent = new GUIContent(verticalJustice, "Vertical Justice.\nTry to fill the full height with content.");
            //else
            //    verticalJusticeContent = new GUIContent("Justice Horizontal");

            //if (myTarget.JusticeVertical)
            //    GUI.color = toggledOnButtonColor;
            //else
            //    GUI.color = toggledOffButtonColor;

            //if (LeftButton(verticalJusticeContent))
            //{
            //    myTarget.JusticeVertical = !myTarget.JusticeVertical;
            //}

            //if (myTarget.JusticeVertical)
            //{
            //    EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(1), GUILayout.MaxWidth(1));
            //    EditorGUILayout.LabelField(new GUIContent("%", "Justice will be only be applied if the elements hold equal/more than the % height"), GUILayout.MinWidth(15), GUILayout.MaxWidth(15));
            //    EditorGUILayout.PropertyField(JusticeVerticalPercent, GUIContent.none, GUILayout.MinWidth(50), GUILayout.MaxWidth(50));
            //}
            //else
            //{
            //EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(1), GUILayout.MaxWidth(1));
            //EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(15), GUILayout.MaxWidth(15));
            //EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(50), GUILayout.MaxWidth(50));
            //}
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            JusticeHorizontal();
            GUILayout.EndHorizontal();

            GUI.color = originalColor;
        }

        private void JusticeHorizontal()
        {
            GUIContent content;

            if (justiceHorizontalTexture)
                content = new GUIContent(justiceHorizontalTexture, "Horizontal Justice.\nTry to fill the full width with content.");
            else
                content = new GUIContent("Justice Horizontal");

            if (myTarget.JusticeHorizontal)
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;

            if (MidButton(content))
            {
                Undo.RecordObject(myTarget, "Update layout");
                myTarget.JusticeHorizontal = !myTarget.JusticeHorizontal;
            }

            if (myTarget.JusticeHorizontal)
            {
                GUI.color = toggledOnButtonColor;
                EditorGUILayout.LabelField(new GUIContent("%", "Justice will be only be applied if the elements hold equal/more than the % width"), GUILayout.MinWidth(15), GUILayout.MaxWidth(15));
                EditorGUILayout.PropertyField(justiceHorizontalPercent, GUIContent.none, GUILayout.MinWidth(50), GUILayout.MaxWidth(50));
            }
            else
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(15), GUILayout.MaxWidth(15));
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MinWidth(50), GUILayout.MaxWidth(50));
            }
        }

        private bool IsHorizontallyLeft()
        {
            if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.LowerLeft)
                return true;
            return false;
        }

        private bool IsHorizontallyCentered()
        {
            if (myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.MiddleCenter || myTarget.Anchor == Alignment.LowerCenter)
                return true;
            return false;
        }

        private bool IsHorizontallyRight()
        {
            if (myTarget.Anchor == Alignment.UpperRight || myTarget.Anchor == Alignment.MiddleRight || myTarget.Anchor == Alignment.LowerRight)
                return true;
            return false;
        }

        private bool IsVerticallyUp()
        {
            if (myTarget.Anchor == Alignment.UpperLeft || myTarget.Anchor == Alignment.UpperCenter || myTarget.Anchor == Alignment.UpperRight)
                return true;
            return false;
        }

        private bool IsVerticallyMiddle()
        {
            if (myTarget.Anchor == Alignment.MiddleLeft || myTarget.Anchor == Alignment.MiddleCenter || myTarget.Anchor == Alignment.MiddleRight)
                return true;
            return false;
        }

        private bool IsVerticallyBottom()
        {
            if (myTarget.Anchor == Alignment.LowerLeft || myTarget.Anchor == Alignment.LowerCenter || myTarget.Anchor == Alignment.LowerRight)
                return true;
            return false;
        }

        private void DrawSize()
        {
            if (myTarget.GetComponent<RectTransform>())
                return;

            float labelWidth = EditorGUIUtility.labelWidth;

            GUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 50;
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(height);
            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = labelWidth;
        }

        private void DrawDebug()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDebug.target = EditorGUILayout.Foldout(showDebug.target, "Debug", true, foldOutStyle);
            GUILayout.EndVertical();

            if (EditorGUILayout.BeginFadeGroup(showDebug.faded))
            {
                //DrawUILine(blueFaded);
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(lines);
                EditorGUILayout.PropertyField(bounds);

                GUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        private void FindProperties()
        {
            autoItemSize = soTarget.FindProperty("autoItemSize");

            //justiceHorizontal = soTarget.FindProperty("_justiceHorizontal");
            //justiceVertical = soTarget.FindProperty("_justiceVertical");
            justiceHorizontalPercent = soTarget.FindProperty("_justiceHorizontalPercent");
            //JusticeVerticalPercent = soTarget.FindProperty("_justiceVerticalPercent");
            spacing = soTarget.FindProperty("_spacing");
            width = soTarget.FindProperty("_width");
            height = soTarget.FindProperty("_height");
            lines = soTarget.FindProperty("lines");
            bounds = soTarget.FindProperty("bounds");
            lineSpacingStyle = soTarget.FindProperty("_lineSpacingStyle");
            verticalOverflow = soTarget.FindProperty("_verticalOverflow");
            elementUpdater = soTarget.FindProperty("elementUpdater");

            alwaysUpdateInPlayMode = soTarget.FindProperty("alwaysUpdateInPlayMode");
            alwaysUpdateBounds = soTarget.FindProperty("alwaysUpdateBounds");

            showDebug = new AnimBool(false);
            showDebug.valueChanged.AddListener(Repaint);
        }

        private void GenerateStyle()
        {
#if MODULAR_3D_TEXT
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
#endif

            if (toggleStyle == null)
            {
                toggleStyle = new GUIStyle(GUI.skin.button);
                toggleStyle.margin = new RectOffset(0, 0, toggleStyle.margin.top, toggleStyle.margin.bottom);
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
                defaultLabel.normal.textColor = ThemeBasedLabelColor();
            }

            EditorStyles.popup.fontSize = 11;
            EditorStyles.popup.fixedHeight = 18;
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
    }
}