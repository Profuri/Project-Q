using TinyGiantStudio.EditorHelpers;
using UnityEditor;
#if ENABLE_INPUT_SYSTEM
#endif

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(ButtonInputProcessor))]
    public class ButtonInputProcessorEditor : Editor
    {
        ButtonInputProcessor myTarget;
        SerializedObject soTarget;

        SerializedProperty tickRate;
        SerializedProperty tickRateSideWays;


        void OnEnable()
        {
            myTarget = (ButtonInputProcessor)target;
            soTarget = new SerializedObject(target);

            GetReferences();
        }

        public override void OnInspectorGUI()
        {
            soTarget.Update();
            //GenerateStyle();
            EditorGUI.BeginChangeCheck();

            //if (!myTarget.GetComponent<ButtonInputSystemGlobal>())
            //{
            //DrawDefaultInspector();
            //}
            //else
            //{
            MText_Editor_Methods.ItalicHorizontalField(tickRate, "Tick Rate", "How long you have to press an arrow key down for it to register as a second key press", FieldSize.extraLarge);
            MText_Editor_Methods.ItalicHorizontalField(tickRateSideWays, "Tick Rate SideWays", "For left and right arrow, How long you have to press an arrow key down for it to register as a second key press", FieldSize.extraLarge);

            //}


            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
                EditorUtility.SetDirty(myTarget);
            }
        }





        void GetReferences()
        {
            tickRate = soTarget.FindProperty("tickRate");
            tickRateSideWays = soTarget.FindProperty("tickRateSideWays");
        }

    }
}