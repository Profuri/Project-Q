using ManagingSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : BaseManager<StageManager>
{
    public Stage CurrentStage { get; private set; }
    public Stage NextStage { get; private set; }

    [Header("Stage Generate Setting")] 
    [SerializeField] private float _stageIntervalDistance;

    [Header("Bridge Setting")]
    [SerializeField] private float _bridgeWidth;
    [SerializeField] private float _bridgeIntervalDistance;

    private Transform stageTrmMain;

    private List<BridgeObject> _bridgeObjects;

    public override void StartManager()
    {
        CurrentStage = null;

        StartStage(0);
    }        

    public override void UpdateManager()
    {
    }

    public void GenerateNextStage(ChapterType chapter, int stage)
    {
        var dir = (NextStage.StageEnterPoint - CurrentStage.StageExitPoint).normalized;
        var exitPoint = CurrentStage.CenterPosition + CurrentStage.StageExitPoint;
        var enterPoint = exitPoint + (dir * _stageIntervalDistance);
        var nextStageCenter = enterPoint + 
            (-NextStage.StageEnterPoint.normalized * NextStage.StageEnterPoint.magnitude);
        
        GenerateBridge(exitPoint, enterPoint);
        
        NextStage = PoolManager.Instance.Pop($"{chapter.ToString()}_Stage_{stage.ToString()}") as Stage;
        NextStage.GenerateStage(nextStageCenter);
    }
    
    public void ChangeStage()
    {
        CurrentStage?.DisappearStage();
    }

    private void GenerateBridge(Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(BridgeRemoveRoutine(0.1f));
    }

    private void RemoveBridge()
    {
        StartCoroutine(BridgeRemoveRoutine(0.1f));
    }

    private IEnumerator BridgeGenerateRoutine(Vector3 startPoint, Vector3 endPoint, float delay)
    {
        var bridgeCount = (endPoint - startPoint).magnitude / 
        foreach (var bridge in _bridgeObjects)
        {
            bridge.Generate();
            yield return new WaitForSeconds(delay);
        }
    }
    
    private IEnumerator BridgeRemoveRoutine(float delay)
    {
        foreach (var bridge in _bridgeObjects)
        {
            bridge.Remove();
            yield return new WaitForSeconds(delay);
        }
    }

    public void StageClear()
    {
        // //CameraManager.Instance.ChangeCamera(EAxisType.NONE);
        // GameManager.Instance.Player.SetEnableInput(false);
        // GameManager.Instance.Player.ConvertDimension(EAxisType.NONE);
        //
        // if (_currentStage.IsEndStage)
        // {
        //     EndChapter();
        //     return;
        // }
        // _currentStage?.GoNext();
        // //SetStageNext();
    }

    public void SetStageNext()
    {
        // _currentStage = _currentStage.NextStage;
    }

    private void EndChapter()
    {
        Debug.Log("Chapter Clear!");
    }

    public void StartStage(int chapter)
    {
        // stageTrmMain = GameObject.Find("StageTrmMain").transform;
        //
        // // List<Stage> tempstages = _chapterDataSO.Chapters[chapter].Stages;
        // tempstages.ForEach((stage) =>
        // {
        //     Stage newStage = Instantiate(stage);
        //     newStage.gameObject.SetActive(false);
        //     stages.Add(newStage);
        // }); 
        //
        //
        // for (int i = 0; i < stages.Count; i++)
        // {
        //     if(i != 0)
        //     {
        //         Transform playerTrm = stages[i].transform.Find("Player");
        //         if(playerTrm != null)
        //             Destroy(stages[i].transform.Find("Player").gameObject);
        //         stages[i].PrevStage = stages[i - 1];
        //     }
        //     
        //     stages[i].IsPlayerEnter = false;
        //     stages[i].CurStageNum = i;
        //
        //     stages[i].transform.SetParent(stageTrmMain.GetChild(i));
        //     stages[i].transform.localPosition = Vector3.zero;
        //     stages[i].transform.localRotation = Quaternion.identity;
        //     
        //     if (i == stages.Count - 1)
        //     {
        //         stages[i].IsEndStage = true;
        //         stages[i].NextStage = null;
        //         break;
        //     }
        //
        //     stages[i].IsEndStage = false;
        //     stages[i].NextStage = stages[i + 1];
        // }
        //
        // _currentStage = stages[0];
        // _currentStage.IsPlayerEnter = true;
        // _currentStage.gameObject.SetActive(true);
    }

    public void ReleaseStage()
    {
        _currentStage = null;
        stages.Clear();
        stages = null;
    }
}
