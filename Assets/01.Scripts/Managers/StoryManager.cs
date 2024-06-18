using System.Collections.Generic;
using UnityEngine;
using ManagingSystem;
using System;
using UnityEngine.Video;
using Unity.VisualScripting;

public class StoryManager : BaseManager<StoryManager>,IProvideSave
{
    private MessageWindow _messagePanel;
    public bool IsPlay {get; private set; }

    private MessageVideoWindow _messageVideoWindow;
    public bool IsPlayMessageVideo => IsPlay && _messageVideoWindow is not null;
    
    public StoryData CurrentPlayStoryData => IsPlay ? _messagePanel.StoryData : null;

    [SerializeField] private List<StoryInfo> _storyList = new List<StoryInfo>();

    public event Action OnStoryReleased = null;

    public override void Init()
    {
        base.Init();
        DataManager.Instance.SettingDataProvidable(this, null);
        IsPlay = false;
    }

    public override void StartManager()
    {
        for (var i = 0; i < _storyList.Count; i++)
        {
            _storyList[i].index = i;
        }
        DataManager.Instance.SaveData(this);
    }

    public void StartStory(StoryData storyData)
    {
        if (IsPlay)
        {
            return;
        }

        IsPlay = true;

        InputManager.Instance.SetEnableInputAll(false);
        UIManager.Instance.Interact3DButton = false;

        _messagePanel = UIManager.Instance.GenerateUI("MessageWindow", null, () =>
        {
            _messagePanel.SetData(storyData);
        }) as MessageWindow;
        DataManager.Instance.SaveData(this);
    }

    public void ReleaseStory()
    {
        if (!IsPlay)
        {
            return;
        }

        StopMessageVideo();
        IsPlay = false;

        InputManager.Instance.SetEnableInputAll(true);
        UIManager.Instance.Interact3DButton = true;

        _messagePanel.Disappear();
        _messagePanel = null;
        
        OnStoryReleased?.Invoke();
        OnStoryReleased = null;
    }

    public void PlayMessageVideo(VideoClip clip)
    {
        if (IsPlayMessageVideo)
        {
            _messageVideoWindow.SettingVideo(clip);
            _messageVideoWindow.Play();
            return;
        }

        _messageVideoWindow = UIManager.Instance.GenerateUI("MessageVideoWindow") as MessageVideoWindow;
        _messageVideoWindow.SettingVideo(clip);
        _messageVideoWindow.Play();
    }

    public void StopMessageVideo()
    {
        if (!IsPlayMessageVideo)
        {
            return;
        }

        _messageVideoWindow.Disappear(() =>
        {
            _messageVideoWindow = null;
        });
    }

    public bool StartStoryIfCan(StoryAppearType appearType, params object[] objs)
    {
        if (IsPlay)
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
