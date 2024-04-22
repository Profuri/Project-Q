using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManagingSystem;
using System;
using System.Linq;

public enum ChapterCondition
{
    CHAPTER_ENTER = 0,
    CHAPTER_EXIT,
    CHAPTER_CLEAR,
}

public class StoryManager : BaseManager<StoryManager>,IProvideSave
{
    public StoryPanel CurrentPanel { get; private set; }    
    public Canvas StoryCanvas { get; private set; }
    
    [System.Serializable]
    private class ChapterStory
    {
        public ChapterCondition condition;
        public ChapterType chapterType;
        public int stageIndex;

        [HideInInspector] public bool isShown;

        public StorySO storySO;
    }

    [SerializeField] private List<ChapterStory> _storyList = new List<ChapterStory>();

    private Tuple<ChapterStory,int> GetStory(ChapterCondition condition,ChapterType chapterType,int stageIndex = 7)
    {
        Predicate<ChapterStory> predicate = (cs) => cs.stageIndex == stageIndex && cs.chapterType == chapterType && cs.condition == condition;
        int index = _storyList.FindIndex(0,_storyList.Count,predicate);
        return Tuple.Create(_storyList[index],index);
    }


    public override void StartManager()
    {

    }

    public void ResetMessage()
    {
        CurrentPanel = null;
    }

    public void StartStory(StorySO storySO,bool isTypingStory = false)
    {
        if (CurrentPanel != null) return;

        CurrentPanel = UIManager.Instance.GenerateUI("StoryPanel") as StoryPanel;
        CurrentPanel.ResetPosition();
        CurrentPanel.SettingStory(storySO,isTypingStory);
        DataManager.Instance.SaveData(this);
    }

    public bool StartStoryIfCan(ChapterCondition condition, ChapterType chapterType, int stageIndex = 7)
    {
        var (chapterStory, index)= GetStory(condition,chapterType,stageIndex);
        if (chapterStory.storySO == null) return false;

        _storyList[index].isShown = true;

        StartStory(chapterStory.storySO, true);
        return true;
    }

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            for(int i =0; i < _storyList.Count; i++)
            {
                saveData.StoryShowList[i] = _storyList[i].isShown;
            }
        };
    }
}
