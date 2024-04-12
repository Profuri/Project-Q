using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CameraShakeBehaviour))]
public class CameraShakeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 2;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty intensityProp = property.FindPropertyRelative("intensity");
        SerializedProperty frequencyProp = property.FindPropertyRelative("frequency");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, intensityProp);
        
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, frequencyProp);
    }
}