using System.Collections.Generic;
using UnityEngine;
using ManagingSystem;
using System;

public class StoryManager : BaseManager<StoryManager>,IProvideSave
{
    private MessageWindow _messagePanel;
    public bool IsPlay => _messagePanel is not null;

    [SerializeField] private List<StoryInfo> _storyList = new List<StoryInfo>();

    public override void StartManager()
    {
        for (var i = 0; i < _storyList.Count; i++)
        {
            _storyList[i].index = i;
        }
        
        DataManager.Instance.SettingDataProvidable(this, null);
    }

    public void StartStory(StoryData storyData)
    {
        if (_messagePanel != null)
        {
            return;
        }

        InputManager.Instance.SetEnableInputAll(false);
        _messagePanel = UIManager.Instance.GenerateUI("MessageWindow", null, () =>
        {
            _messagePanel.SetData(storyData);
        }) as MessageWindow;
        DataManager.Instance.SaveData(this);
    }

    public void ReleaseStory()
    {
        if (_messagePanel == null)
        {
            return;
        }
        
        InputManager.Instance.SetEnableInputAll(true);
        _messagePanel.Disappear();
        _messagePanel = null;
    }

    public bool StartStoryIfCan(StoryAppearType appearType, params object[] objs)
    {
        if (_messagePanel != null)
        {
            return false;
        }
        
        StoryInfo info = null;
        
        if (objs.Length == 1)
        {
            if (objs[0] is SceneType sceneType)
            {
                info = GetStory(appearType, sceneType);
            }
            else if (objs[0] is TimelineType timelineType)
            {
                info = GetStory(appearType, timelineType);
            }
        }
        else if (objs.Length == 2)
        {
            info = GetStory(appearType, (ChapterType)objs[0], (int)objs[1]);
        }

        if (info == null)
        {
            return false;
        }

        if (info.recordPlayed)
        {
            DataManager.sSaveData.StoryShowList[info.index] = true;
        }
        StartStory(info.storyData);
        return true;
    }

    private StoryInfo GetStory(StoryAppearType appearType, SceneType sceneType)
    {
        var index = _storyList.FindIndex(story => story.Predicate(appearType, sceneType));
        if (index != -1 && !DataManager.sSaveData.StoryShowList[index])
        {
            return _storyList[index];
        }
        return null;
    }
    
    private StoryInfo GetStory(StoryAppearType appearType, TimelineType timelineType)
    {
        var index = _storyList.FindIndex(story => story.Predicate(appearType, timelineType));
        if (index != -1 && !DataManager.sSaveData.StoryShowList[index])
        {
            return _storyList[index];
        }
        return null;
    }

    private StoryInfo GetStory(StoryAppearType appearType, ChapterType chapterType, int stageIndex)
    {
        var index = _storyList.FindIndex(story => story.Predicate(appearType, chapterType, stageIndex));
        if (index != -1 && !DataManager.sSaveData.StoryShowList[index])
        {
            return _storyList[index];
        }
        return null;
    }

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            for(int i = 0; i < _storyList.Count; i++)
            {
                if(saveData.StoryShowList.Count <= i)
                {
                    saveData.StoryShowList.Add(false);
                }
            }
        };
    }
}
