using System;
using UnityEngine;

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

    public bool Predicate(StoryAppearType appearType, SceneType sceneType)
    {
        return this.appearType == appearType && this.sceneType == sceneType;
    }
    
    public bool Predicate(StoryAppearType appearType, TimelineType timelineType)
    {
        return this.appearType == appearType && this.timelineType == timelineType;
    }
    
    public bool Predicate(StoryAppearType appearType, ChapterType chapterType, int stageIndex)
    {
        return this.appearType == appearType && this.chapterType == chapterType && this.stageIndex == stageIndex;
    }
}