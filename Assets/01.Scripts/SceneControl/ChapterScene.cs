using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class ChapterScene : Scene, IProvideLoad
{
    private Dictionary<ChapterType, Chapter> _chapterDictionary;

    public UnityEvent<ChapterType,SaveData> OnChapterClear;
    public UnityEvent<bool> OnSubChaptersClear;

    private PlayableDirector _timelineDirector;
    
    protected override void Awake()
    {
        base.Awake();
        _timelineDirector = GetComponent<PlayableDirector>();
        _chapterDictionary = new Dictionary<ChapterType, Chapter>();
        var chapters = GetComponentsInChildren<Chapter>();
        foreach( var chapter in chapters )
        {
            ChapterType chapterType = chapter.Data.chapter;
            _chapterDictionary.Add(chapterType,chapter);
        }
        LoadToDataManager();
    }

    public override void OnPop()
    {
        base.OnPop();
        DataManager.Instance.LoadData(this);
    }
    
    private void ShowChapterClearTimeline(ChapterType clearChapter, bool skipOnStart = false, Action onComplete = null)
    {
        var type = (TimelineType)Enum.Parse(typeof(TimelineType), $"{clearChapter.ToString()}Clear");
        TimelineManager.Instance.ShowTimeline(_timelineDirector, type, skipOnStart, onComplete);
    }

    private void LoadChapterEvents(SaveData saveData)
    {
        if (saveData == null)
            return;
        
        var clearDictionary = saveData.ChapterClearDictionary;
        var clearCnt = 0;
        
        foreach(var (chapterType, isClear) in clearDictionary)
        {
            if (!isClear)
            {
                continue;
            }
            
            clearCnt++;
            OnChapterClear?.Invoke(chapterType,saveData);

            var alreadyShowCutScene = saveData.ChapterTimelineDictionary[chapterType]; 
                
            if (!alreadyShowCutScene)
            {
                saveData.ChapterTimelineDictionary[chapterType] = true;
                DataManager.Instance.SaveData();
            }

            ShowChapterClearTimeline(chapterType, alreadyShowCutScene);
        }

        if (clearCnt > 4)
        {
            TimelineManager.Instance.ShowTimeline(_timelineDirector, TimelineType.AllChapterClear);
            OnSubChaptersClear?.Invoke(true);
            DataManager.Instance.SaveData();
        }
    }

    public Action<SaveData> GetLoadAction()
    {
        return LoadChapterEvents;
    }
    
    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(null,this);
    }
}
