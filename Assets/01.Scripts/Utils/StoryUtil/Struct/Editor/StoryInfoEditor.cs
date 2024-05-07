using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StoryInfo))]
public class StoryInfoEditor : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var appearTypeProperty = property.FindPropertyRelative("appearType");
        var appearType = (StoryAppearType)appearTypeProperty.enumValueIndex;
        int cnt = 0;
        
        switch (appearType)
        {
            case StoryAppearType.SCENE_ENTER: cnt = 3; break;
            case StoryAppearType.CUTSCENE_END: cnt = 3; break;
            case StoryAppearType.STAGE_ENTER:
            case StoryAppearType.STAGE_EXIT: cnt = 4; break;
        }
        
        return 20f * cnt;
    }
    
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var appearTypeProperty = property.FindPropertyRelative("appearType");

        var sceneTypeProperty = property.FindPropertyRelative("sceneType");
        var timelineTypeProperty = property.FindPropertyRelative("timelineType");
        var chapterTypeProperty = property.FindPropertyRelative("chapterType");
        var stageIndexProperty = property.FindPropertyRelative("stageIndex");
        
        var storyDataProperty = property.FindPropertyRelative("storyData");

        EditorGUIUtility.wideMode = true;
        EditorGUIUtility.labelWidth = 100;
        
        var appearType = (StoryAppearType)appearTypeProperty.enumValueIndex;
        switch (appearType)
        {
            case StoryAppearType.SCENE_ENTER: rect.height /= 3; break;
            case StoryAppearType.CUTSCENE_END: rect.height /= 3; break;
            case StoryAppearType.STAGE_ENTER:
            case StoryAppearType.STAGE_EXIT: rect.height /= 4; break;
        }

        appearTypeProperty.enumValueIndex = Convert.ToInt32(EditorGUI.EnumPopup(rect, "AppearType", (StoryAppearType)appearTypeProperty.enumValueIndex));
        rect.y += rect.height;
        
        appearType = (StoryAppearType)appearTypeProperty.enumValueIndex;
        switch (appearType)
        {
            case StoryAppearType.SCENE_ENTER:
            {
                sceneTypeProperty.enumValueIndex = Convert.ToInt32(EditorGUI.EnumPopup(rect, "SceneType",
                    (SceneType)sceneTypeProperty.enumValueIndex));
                break;
            }
            case StoryAppearType.CUTSCENE_END:
            {
                timelineTypeProperty.enumValueIndex = Convert.ToInt32(EditorGUI.EnumPopup(rect, "TimelineType",
                    (ChapterType)timelineTypeProperty.enumValueIndex));
                break;
            }
            case StoryAppearType.STAGE_ENTER:
            case StoryAppearType.STAGE_EXIT:
            {
                chapterTypeProperty.enumValueIndex = Convert.ToInt32(EditorGUI.EnumPopup(rect, "ChapterType",
                    (ChapterType)chapterTypeProperty.enumValueIndex));                
                rect.y += rect.height;
                stageIndexProperty.intValue = EditorGUI.IntField(rect, "StageIndex", stageIndexProperty.intValue);
                break;
            }
        }
        
        rect.y += rect.height;
        storyDataProperty.objectReferenceValue = EditorGUI.ObjectField(rect, "StoryData", storyDataProperty.objectReferenceValue, typeof(StoryData));
    }
}