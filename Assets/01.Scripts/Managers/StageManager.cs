using ManagingSystem;
using AxisConvertSystem;
using UnityEngine;
using System;


public class StageManager : BaseManager<StageManager>,IDataProvidable
{
    public Stage CurrentStage { get; private set; }
    public Stage NextStage { get; private set; }

    private ChapterData _currentPlayChapterData;

    public AxisType CurrentStageAxis => SceneControlManager.Instance.Player.Converter.AxisType;
    public override void StartManager()
    {
        CurrentStage = null;
        NextStage = null;
    }        

    public void StartNewChapter(ChapterData chapterData)
    {
        _currentPlayChapterData = chapterData;
        if (chapterData.stageCnt < 1)
        {
            Debug.LogError($"StageCnt: {1}");
            DataManager.Instance.SaveData(this);
        }
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
            Debug.Log("this is a last stage chapter clear!!!");
            DataManager.Instance.SaveData(this);
            return;
        }
        
        GenerateNextStage(_currentPlayChapterData.chapter, nextChapter);
    }
    

    public Action<SaveData> GetProvideAction()
    {
        return (saveData) =>
        {
            var currentChapter = _currentPlayChapterData.chapter;
            bool isClear = CurrentStage.stageOrder + 1 >= _currentPlayChapterData.stageCnt;

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
}
