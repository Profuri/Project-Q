using ManagingSystem;
using AxisConvertSystem;
using UnityEngine;
using System;


public class StageManager : BaseManager<StageManager>, IDataProvidable
{
    public Stage CurrentStage { get; private set; }
    public Stage NextStage { get; private set; }

    private ChapterData _currentPlayChapterData;

    public AxisType CurrentStageAxis => SceneControlManager.Instance.Player.Converter.AxisType;
    
    public override void StartManager()
    {
        CurrentStage = null;
        NextStage = null;
        LoadToDataManager();
    }        

    public void StartNewChapter(ChapterData chapterData)
    {
        _currentPlayChapterData = chapterData;
        CurrentStage = SceneControlManager.Instance.AddObject(
            $"{chapterData.chapter.ToString().ToUpperFirstChar()}_Stage_0") as Stage;
        CurrentStage.Generate(Vector3.zero, false);
    }

    public void GenerateNextStage(ChapterType chapter, int stage)
    {
        NextStage = SceneControlManager.Instance.AddObject(
            $"{chapter.ToString().ToUpperFirstChar()}_Stage_{stage.ToString()}") as Stage;
        CurrentStage.ConnectOtherSection(NextStage);
    }

    public void ChangeToNextStage()
    {
        if (CurrentStage is not null)
        {
            CurrentStage.Disappear();
            CurrentStage.RemoveBridge();
        }

        CurrentStage = NextStage;
    }

    public void StageClear(PlayerUnit player)
    {
        CurrentStage.Lock = false;
        player.Converter.SetConvertable(false);
        var nextChapter = CurrentStage.stageOrder + 1;

        if (nextChapter >= _currentPlayChapterData.stageCnt)
        {
            //DataManager.Insta
            DataManager.Instance.SaveData(this);
            return;
        }
        
        GenerateNextStage(_currentPlayChapterData.chapter, nextChapter);
    }
    

    public Action<SaveData> GetProvideAction()
    {
        return (saveData) =>
        {
            if (_currentPlayChapterData == null) return;
            var currentChapter = _currentPlayChapterData.chapter;
            bool isClear = CurrentStage.stageOrder + 1 >= _currentPlayChapterData.stageCnt;

            Debug.Log($"SaveData: {saveData}");
            Debug.Log($"ChapterProgressDictionary: {saveData.ChapterProgressDictionary}");

            if (saveData.ChapterProgressDictionary.ContainsKey(currentChapter) == false)
            {
                saveData.ChapterProgressDictionary.Add(currentChapter, isClear);
            }
            else
            {
                saveData.ChapterProgressDictionary[currentChapter] = isClear;
            }
        };
    }

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            Debug.Log("ClearChapter");
        };
    }

    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(this);
    }
}
