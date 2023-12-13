using ManagingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseManager<StageManager>
{
    [SerializeField] private ChapterDataSO _chapterDataSO;
    public ChapterDataSO GetChapterData => _chapterDataSO;

    private Stage _curStage;
    public Stage CurStage => _curStage;

    public int CurChapterNum = 0;

    public override void StartManager()
    {
        _curStage = null;

        for (int i = 0; i < _chapterDataSO.Chapters.Count; i++)
        {
            List<Stage> stages = _chapterDataSO.Chapters[i].Stages;

            for (int j = 0; j < stages.Count; j++)
            {
                Stage stage = stages[j];
                stage.CurStageNum = j;

                //챕터별 마지막 스테이지
                if (j == stages.Count - 1)
                {
                    stage.IsEndStage = true;
                    stage.NextStage = null;
                    break;
                }

                stage.IsEndStage = false;
                stage.NextStage = stages[j + 1];
            }
        }
    }        

    public override void UpdateManager()
    {
                
    }

    public void StageClear()
    {
        if(_curStage.IsEndStage)
        {
            EndChapter();
            return;
        }
        _curStage?.GoNext();
        _curStage = _curStage.NextStage;    
    }

    private void EndChapter()
    {
        Debug.Log("Chapter Clear!");
    }

    public void SetInitStage(int chapter)
    {
        _curStage = _chapterDataSO.Chapters[chapter].Stages[0];
    }
}
