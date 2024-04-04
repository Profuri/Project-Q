using ManagingSystem;
using AxisConvertSystem;
using UnityEngine;
using System;

public class StageManager : BaseManager<StageManager>, IProvideSave 
{
    public Stage CurrentStage { get; private set; }
    public Stage NextStage { get; private set; }

    public bool IsClear { get; private set; }

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
        IsClear = false;

        _currentPlayChapterData = chapterData;
        CurrentStage = SceneControlManager.Instance.AddObject(
            $"{chapterData.chapter.ToString().ToUpperFirstChar()}_Stage_0") as Stage;
        CurrentStage.Generate(Vector3.zero, false, 1.5f);
    }

    public void GenerateNextStage(ChapterType chapter, int stage)
    {
        NextStage = SceneControlManager.Instance.AddObject(
            $"{chapter.ToString().ToUpperFirstChar()}_Stage_{stage.ToString()}") as Stage;
        CurrentStage.ConnectOtherSection(NextStage);
    }

    public void RestartStage(PlayerUnit player)
    {
        if(CurrentStage != null && !IsClear)
        {
            string stageName = CurrentStage.gameObject.name;
            string playerName = player.gameObject.name;
            Vector3 currentPos = CurrentStage.CenterPosition;

            player.useGravity = false;

            const float dissolveTime = 0.80623f;
            player.Dissolve(1f, dissolveTime);


            CurrentStage.Disappear(dissolveTime, () =>
            {
                CurrentStage = SceneControlManager.Instance.AddObject(stageName) as Stage;
                CurrentStage.Generate(currentPos, true, dissolveTime, null);
                player.SetSection(CurrentStage);
                player.useGravity = true;
                player.ReloadUnit(dissolveTime);
            });
        }
    }
    public void ChangeToNextStage()
    {
        if (CurrentStage is not null)
        {
            CurrentStage.Disappear();
            CurrentStage.RemoveBridge();
        }

        IsClear = false;
        CurrentStage = NextStage;
    }

    public void StageClear(PlayerUnit player)
    {
        IsClear = true;
        CurrentStage.Lock = false;
        player.Converter.SetConvertable(false);
        var nextChapter = CurrentStage.stageOrder + 1;

        if (nextChapter >= _currentPlayChapterData.stageCnt)
        {
            DataManager.Instance.SaveData(this);
            SceneControlManager.Instance.LoadScene(SceneType.Chapter);
            return;
        }
        
        GenerateNextStage(_currentPlayChapterData.chapter, nextChapter);
    }
    

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            if (_currentPlayChapterData == null) return;
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

    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(this,null);
    }
}
