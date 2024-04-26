using System.Collections.Generic;
using UnityEngine;
using ManagingSystem;
using System;

public enum ChapterCondition
{
    CHAPTER_ENTER = 0,
    CHAPTER_EXIT,
    CHAPTER_CLEAR,
}

public class StoryManager : BaseManager<StoryManager>,IProvideSave,IProvideLoad
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
        bool Predicate(ChapterStory cs)
        {
            return cs.stageIndex == stageIndex && cs.chapterType == chapterType && cs.condition == condition;
        }
        
        int index = _storyList.FindIndex(0, _storyList.Count, Predicate);
        if (index != -1 && !_storyList[index].isShown)
        {
            return Tuple.Create(_storyList[index],index);
        }
        
        return null;
    }

    public override void StartManager()
    {
        DataManager.Instance.SettingDataProvidable(this, this);
        DataManager.Instance.LoadData(this);
    }

    public void ResetMessage()
    {
        CurrentPanel = null;
    }

    public void StartStory(StorySO storySO,int storyIndex = 0,bool isTypingStory = false)
    {
        if (CurrentPanel != null) return;

        CurrentPanel = UIManager.Instance.GenerateUI("StoryPanel") as StoryPanel;
        CurrentPanel.ResetPosition();
        CurrentPanel.SettingStory(storySO,isTypingStory);
        DataManager.Instance.SaveData(this);
    }

    public bool StartStoryIfCan(ChapterCondition condition, ChapterType chapterType, int stageIndex = 7)
    {
        var tuple = GetStory(condition,chapterType,stageIndex);

        if (tuple == null) return false;

        StorySO storySO = tuple.Item1.storySO;
        int index = tuple.Item2;

        _storyList[index].isShown = true;

        StartStory(storySO,index, true);
        return true;
    }


    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            for(int i =0; i < _storyList.Count; i++)
            {
                if(saveData.StoryShowList.Count <= i)
                {
                    saveData.StoryShowList.Add(_storyList[i].isShown);
                }
                saveData.StoryShowList[i] = _storyList[i].isShown;
            }
        };
    }

    public Action<SaveData> GetLoadAction()
    {
        return (saveData) =>
        {
            for (int i = 0; i < _storyList.Count; i++)
            {
                try
                {
                    _storyList[i].isShown = saveData.StoryShowList[i];
                }
                catch
                {
                    _storyList[i].isShown = false;
                }
            }
        };
    }
}
