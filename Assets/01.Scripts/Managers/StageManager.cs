using ManagingSystem;
using AxisConvertSystem;
using UnityEngine;
using System;

public class StageManager : BaseManager<StageManager>, IProvideSave,IProvideLoad
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
        CursorManager.SetCursorEnable(false);
        CursorManager.SetCursorLockState(CursorLockMode.Locked);

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

    public void RestartStage(PlayerUnit player)
    {
        if(CurrentStage != null)
        {
            string stageName = CurrentStage.gameObject.name;
            Vector3 currentPos = CurrentStage.CenterPosition;

            SceneControlManager.Instance.DeleteObject(player);
            CurrentStage.Disappear(() =>
            {
                CurrentStage = SceneControlManager.Instance.AddObject(stageName) as Stage;
                //위치 설정
                CurrentStage.Generate(currentPos, true);
                SceneControlManager.Instance.AddObject(player.name);
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

    public Action<SaveData> GetLoadAction()
    {
        return (saveData) =>
        {
            Debug.Log("ClearChapter");
        };
    }

    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(this,this);
    }
}
