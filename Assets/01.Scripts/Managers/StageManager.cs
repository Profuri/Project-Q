using ManagingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseManager<StageManager>
{
    public Stage CurrentStage { get; private set; }
    public Stage NextStage { get; private set; }

    public EAxisType CurrentStageAxis => CurrentStage.Converter.AxisType;

    [Header("Chapter Data")] 
    [SerializeField] private ChapterDataSO _chapterData;

    [Header("For Debugging")] 
    [SerializeField] private bool _generateStageOnStart;
    [SerializeField] private ChapterType _startChapter;
    [SerializeField] private int _startStage;

    public override void StartManager()
    {
        CurrentStage = null;
        NextStage = null;

        if (_generateStageOnStart)
        {
            NextStage = PoolManager.Instance.Pop($"{_startChapter.ToString()}_Stage_{_startStage.ToString()}") as Stage;
            NextStage.Generate(Vector3.zero);
            PoolManager.Instance.Pop("Player");
        }
    }        

    public override void UpdateManager()
    {
        // Do nothing
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
        player.SetStage(CurrentStage);
        CurrentStage.InitStage();
    }

    public void StageClear()
    {
        CurrentStage.Converter.ConvertDimension(EAxisType.NONE);

        var curChapter = CurrentStage.Chapter;
        var nextChapter = CurrentStage.stageOrder + 1;

        if (nextChapter >= _chapterData.GetStageCnt(curChapter))
        {
            curChapter++;
            nextChapter = 0;

            if (curChapter >= ChapterType.CNT)
            {
                Debug.Log("this is a last stage game clear!!!");
                return;
            }
        }
        
        GenerateNextStage(curChapter, nextChapter);
    }
}
