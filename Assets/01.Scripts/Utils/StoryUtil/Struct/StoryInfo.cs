using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class StoryInfo
{
    public StoryAppearType appearType;
    
    // enter scene
    public SceneType sceneType;
    
    // timeline end
    public TimelineType timelineType;
    
    // stage enter or exit
    public ChapterType chapterType;
    public int stageIndex;
    
    public StoryData storyData;
    [HideInInspector] public bool isShown;
}