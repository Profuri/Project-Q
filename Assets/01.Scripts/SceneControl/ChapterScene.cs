using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class ChapterScene : Scene, IProvideLoad
{
    [SerializeField] private UnityEvent onAllChapterClear = null;
    
    private PlayableDirector _timelineDirector;
    
    protected override void Awake()
    {
        base.Awake();
        _timelineDirector = GetComponent<PlayableDirector>();
        
        LoadToDataManager();
    }

    public override void OnPop()
    {
        base.OnPop();
        DataManager.Instance.LoadData(this);
    }
    
    private void ShowChapterClearTimeline(SaveData saveData, ChapterType clearChapter, Action onComplete = null)
    {
        var type = (TimelineType)Enum.Parse(typeof(TimelineType), $"{clearChapter.ToString()}Clear");
        var alreadyShowCutScene = saveData.ChapterTimelineDictionary[clearChapter]; 
                
        if (!alreadyShowCutScene)
        {
            saveData.ChapterTimelineDictionary[clearChapter] = true;
        }

        TimelineManager.Instance.ShowTimeline(_timelineDirector, type, alreadyShowCutScene, onComplete);
    }

    private void LoadChapterEvents(SaveData saveData)
    {
        if (saveData == null)
            return;
        
        var clearDictionary = saveData.ChapterClearDictionary;
        var clearCnt = 0;

        foreach(var (chapterType, isClear) in clearDictionary)
        {
            if (chapterType == ChapterType.Cpu || chapterType == ChapterType.Tutorial || !isClear)
            {
                continue;
            }
            
            clearCnt++;
            ShowChapterClearTimeline(saveData, chapterType, null);
        }

        if (clearCnt > 4)
        {
            var alreadyShowCutScene = saveData.ChapterTimelineDictionary[ChapterType.Cpu]; 
                
            if (!alreadyShowCutScene)
            {
                saveData.ChapterTimelineDictionary[ChapterType.Cpu] = true;
            }
            

            TimelineManager.Instance.ShowTimeline(_timelineDirector, TimelineType.AllChapterClear, alreadyShowCutScene);
            onAllChapterClear?.Invoke();
        }
        
        DataManager.Instance.SaveData();
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
