using ManagingSystem;
using AxisConvertSystem;
using UnityEngine;
using System;

public class StageManager : BaseManager<StageManager>, IProvideSave 
{
    public Stage CurrentStage { get; private set; }
    public Stage NextStage { get; private set; }

    public bool IsClear
    {
        get
        {
            if (CurrentStage == null)
            {
                Debug.LogError($"CurrentStage is null");
                return false;
            }
            return CurrentStage.IsClear;
        }
    }

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
        //IsClear = false;

        _currentPlayChapterData = chapterData;
        CurrentStage = SceneControlManager.Instance.AddObject(
            $"{chapterData.chapter.ToString().ToUpperFirstChar()}_Stage_0") as Stage;
        CurrentStage.Generate(Vector3.zero, false, false);
        CurrentStage.IsClear = false;
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
            Vector3 currentPos = CurrentStage.CenterPosition;

            player.useGravity = false;

            const float dissolveTime = 0.80623f;
            player.Dissolve(1f, dissolveTime);

            CurrentStage.Disappear(dissolveTime, () =>
            {
                CurrentStage = SceneControlManager.Instance.AddObject(stageName) as Stage;
                CurrentStage.Generate(currentPos, true, false, dissolveTime);
                player.SetSection(CurrentStage);
                player.useGravity = true;
                player.ReloadUnit(true, dissolveTime);
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

        //sIsClear = false;
        CurrentStage = NextStage;
        CurrentStage.IsClear = false;
    }

    public void StageClear(PlayerUnit player)
    {
        if(CurrentStage is null || IsClear) return;
        //IsClear = true;
        CurrentStage.Lock = false;
        player.OriginUnitInfo.LocalPos = CurrentStage.PlayerResetPointInClear;
        player.Converter.ConvertDimension(AxisType.None);
        player.Converter.SetConvertable(false);
        var nextChapter = CurrentStage.StageOrder + 1;

        if (nextChapter >= _currentPlayChapterData.stageCnt)
        {
            DataManager.Instance.SaveData(this);
            SceneControlManager.Instance.LoadScene(SceneType.Chapter);
            return;
        }
                
        GenerateNextStage(_currentPlayChapterData.chapter, nextChapter);
        CurrentStage.IsClear = true;
    }

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            if (_currentPlayChapterData == null) return;
            var currentChapter = _currentPlayChapterData.chapter;
            bool isClear = CurrentStage.StageOrder + 1 >= _currentPlayChapterData.stageCnt;
            
            saveData.ChapterClearDictionary[currentChapter] = isClear;
        };
    }

    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(this,null);
    }
}
