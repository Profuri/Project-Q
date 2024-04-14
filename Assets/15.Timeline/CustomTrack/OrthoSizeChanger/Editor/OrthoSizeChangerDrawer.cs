using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OrthoSizeChangerBehaviour))]
public class OrthoSizeChangerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 2;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var originSizeProp = property.FindPropertyRelative("originSize");
        var targetSizeProp = property.FindPropertyRelative("targetSize");

        var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, originSizeProp);
        
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, targetSizeProp);
    }   
}