using ManagingSystem;
using System.Collections;
using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;

public class StageManager : BaseManager<StageManager>
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

    public void StageClear()
    {
        CurrentStage.Lock = false;
        var nextChapter = CurrentStage.stageOrder + 1;

        if (nextChapter >= _currentPlayChapterData.stageCnt)
        {
            Debug.Log("this is a last stage chapter clear!!!");
            return;
        }
        
        GenerateNextStage(_currentPlayChapterData.chapter, nextChapter);
    }
}
