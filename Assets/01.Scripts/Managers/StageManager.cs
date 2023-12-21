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

    private Transform stageTrmMain;

    private List<Stage> stages;

    public Vector3 ReloadPoint => CurStage.ReloadPoint;

    public override void StartManager()
    {
        _curStage = null;
        stages = new();

        //Debuging
        StartStage(0);
    }        

    public override void UpdateManager()
    {
    }


    public void StageClear()
    {
        //CameraManager.Instance.ChangeCamera(EAxisType.NONE);
        GameManager.Instance.Player.SetEnableInput(false);
        GameManager.Instance.Player.ConvertDimension(EAxisType.NONE);

        if (_curStage.IsEndStage)
        {
            EndChapter();
            return;
        }
        _curStage?.GoNext();
        //SetStageNext();
    }

    public void SetStageNext()
    {
        _curStage = _curStage.NextStage;
    }

    private void EndChapter()
    {
        Debug.Log("Chapter Clear!");
    }

    public void StartStage(int chapter)
    {
        stageTrmMain = GameObject.Find("StageTrmMain").transform;

        List<Stage> tempstages = _chapterDataSO.Chapters[chapter].Stages;
        tempstages.ForEach((stage) =>
        {
            Stage newStage = Instantiate(stage);
            newStage.gameObject.SetActive(false);
            stages.Add(newStage);
        }); 
       

        for (int i = 0; i < stages.Count; i++)
        {
            if(i != 0)
            {
                Transform playerTrm = stages[i].transform.Find("Player");
                if(playerTrm != null)
                    Destroy(stages[i].transform.Find("Player").gameObject);
                stages[i].PrevStage = stages[i - 1];
            }
            
            stages[i].IsPlayerEnter = false;
            stages[i].CurStageNum = i;

            stages[i].transform.SetParent(stageTrmMain.GetChild(i));
            stages[i].transform.localPosition = Vector3.zero;
            stages[i].transform.localRotation = Quaternion.identity;
            
            if (i == stages.Count - 1)
            {
                stages[i].IsEndStage = true;
                stages[i].NextStage = null;
                break;
            }

            stages[i].IsEndStage = false;
            stages[i].NextStage = stages[i + 1];
        }

        _curStage = stages[0];
        _curStage.IsPlayerEnter = true;
        _curStage.gameObject.SetActive(true);
    }

    public void ReleaseStage()
    {
        _curStage = null;
        stages.Clear();
        stages = null;
    }
}
