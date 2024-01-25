using ManagingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseManager<StageManager>
{
    public Stage CurrentStage { get; private set; }
    public Stage NextStage { get; private set; }

    private ChapterData _currentPlayChapterData;

    // public EAxisType CurrentStageAxis => CurrentStage.Converter.AxisType;

    public override void StartManager()
    {
        CurrentStage = null;
        NextStage = null;
    }        

    public void StartNewChapter(ChapterData chapterData)
    {
        _currentPlayChapterData = chapterData;
        NextStage = PoolManager.Instance.Pop($"{chapterData.chapter.ToString()}_Stage_0") as Stage;
        NextStage.Generate(Vector3.zero);
    }

    public void GenerateNextStage(ChapterType chapter, int stage)
    {
        NextStage = PoolManager.Instance.Pop($"{chapter.ToString()}_Stage_{stage.ToString()}") as Stage;
        CurrentStage.ConnectOtherSection(NextStage);
    }
    
    public void ChangeToNextStage(PlayerController player)
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
