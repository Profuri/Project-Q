using UnityEditor;
using UnityEngine;
using TinyGiantStudio.Modules;

#if MODULAR_3D_TEXT
using TinyGiantStudio.Text;
#endif

using TinyGiantStudio.EditorHelpers;


namespace TinyGiantStudio.Layout
{
    [CustomEditor(typeof(CircularLayoutGroup))]
    public class CircularLayoutGroupEditor : Editor
    {
        CircularLayoutGroup myTarget;
        SerializedObject soTarget;

        SerializedProperty autoItemSize;
        SerializedProperty angle;
        SerializedProperty useAngle;
        SerializedProperty style;

        SerializedProperty spread;
        SerializedProperty radius;
        SerializedProperty radiusDecreaseRate;

        SerializedProperty alwaysUpdateInPlayMode;
        SerializedProperty alwaysUpdateBounds;

        SerializedProperty elementUpdater;

        Texture rightIcon;
        Texture leftIcon;



        void OnEnable()
        {
            myTarget = (CircularLayoutGroup)target;
            soTarget = new SerializedObject(target);

            rightIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Layouts/Utility/Editor Icons/Icon_Right.png") as Texture;
            leftIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Layouts/Utility/Editor Icons/Icon_Left.png") as Texture;

            FindProperties();
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            EditorGUI.BeginChangeCheck();
            //EditorGUILayout.PropertyField(direction, GUIContent.none);

            GUILayout.BeginHorizontal();
            float size = 38;
            if (myTarget.direction != CircularLayoutGroup.Direction.left)
                GUI.color = Color.gray;
            if (GUILayout.Button(leftIcon, GUILayout.Height(size), GUILayout.Width(size))) //need to add undo
            {
                myTarget.direction = CircularLayoutGroup.Direction.left;
                EditorUtility.SetDirty(myTarget);
#if MODULAR_3D_TEXT
                if (myTarget.GetComponent<Modular3DText>())
                    myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                else
#endif
                    myTarget.UpdateLayout();

            }

            GUI.color = Color.white;

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useAngle, GUIContent.none, GUILayout.MaxWidth(15));

            if (!myTarget.useAngle)
                GUI.color = Color.gray;
            EditorGUILayout.PropertyField(angle, GUIContent.none);

            GUILayout.EndHorizontal();
            if (myTarget.useAngle)
                GUI.color = Color.gray;
            else
                GUI.color = Color.white;
            EditorGUILayout.PropertyField(style, GUIContent.none);
            GUILayout.EndVertical();


            if (myTarget.direction != CircularLayoutGroup.Direction.right)
                GUI.color = Color.gray;
            else
                GUI.color = Color.white;

            if (GUILayout.Button(rightIcon, GUILayout.Height(size), GUILayout.Width(size)))   //need to add undo
            {
                myTarget.direction = CircularLayoutGroup.Direction.right;
                EditorUtility.SetDirty(myTarget);
#if MODULAR_3D_TEXT
                if (myTarget.GetComponent<Modular3DText>())
                    myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                else
#endif
                    myTarget.UpdateLayout();

            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(spread);
            EditorGUILayout.PropertyField(radius);
            MText_Editor_Methods.HorizontalField(radiusDecreaseRate, "Radius Decrease Rate", "", FieldSize.extraLarge);
            DrawAutoItemSize();

            if (ModuleDrawer.ElementUpdatersExist())
                ModuleDrawer.ElementUpdaterContainerList("Element Updater", "", myTarget.elementUpdater, elementUpdater, soTarget);

            GUILayout.Space(5);
            MText_Editor_Methods.ItalicHorizontalField(alwaysUpdateInPlayMode, "Always update in playmode", "For performance, it's better to leave it to false and call UpdateLayout() after making changes.\nTurn this on if you are in a hurry or testing stuff.", FieldSize.gigantic);
            MText_Editor_Methods.ItalicHorizontalField(alwaysUpdateBounds, "Always update bounds", "For performance, it's better to leave it to false and call GetAllChildBounds() when a bound(size of an element) changes", FieldSize.gigantic);
            GUILayout.Space(5);
            if (EditorGUI.EndChangeCheck())
            {
                if (soTarget.ApplyModifiedProperties())
                {
#if MODULAR_3D_TEXT
                    if (myTarget.GetComponent<Modular3DText>())
                        myTarget.GetComponent<Modular3DText>().CleanUpdateText();
#endif
                }
                //EditorUtility.SetDirty(myTarget);
            }
        }

        private void DrawAutoItemSize()
        {
#if MODULAR_3D_TEXT
            if (!myTarget.GetComponent<Modular3DText>())
#endif
            {
                EditorGUILayout.PropertyField(autoItemSize);
            }
        }

        void FindProperties()
        {
            autoItemSize = soTarget.FindProperty("autoItemSize");
            useAngle = soTarget.FindProperty("useAngle");
            angle = soTarget.FindProperty("angle");
            style = soTarget.FindProperty("style");

            spread = soTarget.FindProperty("spread");
            radius = soTarget.FindProperty("radius");
            radiusDecreaseRate = soTarget.FindProperty("radiusDecreaseRate");

            alwaysUpdateInPlayMode = soTarget.FindProperty("alwaysUpdateInPlayMode");
            alwaysUpdateBounds = soTarget.FindProperty("alwaysUpdateBounds");

            elementUpdater = soTarget.FindProperty("elementUpdater");
        }
    }
}