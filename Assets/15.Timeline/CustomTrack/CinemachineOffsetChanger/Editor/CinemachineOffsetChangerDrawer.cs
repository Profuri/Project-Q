using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CinemachineOffsetChangerBehaviour))]
public class CinemachineOffsetChangerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 1;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var targetOffsetProp = property.FindPropertyRelative("targetOffset");

        var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, targetOffsetProp);
    }
}
