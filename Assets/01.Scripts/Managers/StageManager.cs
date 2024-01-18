using ManagingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageManager : BaseManager<StageManager>
{
    public Stage CurrentStage { get; private set; }
    public Stage NextStage { get; private set; }

    [Header("Chapter Data")] 
    [SerializeField] private ChapterDataSO _chapterData;

    [Header("Stage Generate Setting")] 
    [SerializeField] private float _stageIntervalDistance;

    [Header("Bridge Setting")] 
    [SerializeField] private float _bridgeGenerateDelay;
    [SerializeField] private float _bridgeWidth;
    [SerializeField] private float _bridgeIntervalDistance;

    [Space(30), Header("For Debugging")] 
    [SerializeField] private bool _generateStageOnStart;
    [SerializeField] private ChapterType _startChapter;
    [SerializeField] private int _startStage;

    private List<BridgeObject> _bridgeObjects;

    public override void StartManager()
    {
        CurrentStage = null;
        NextStage = null;
        _bridgeObjects = new List<BridgeObject>();

        if (_generateStageOnStart)
        {
            NextStage = PoolManager.Instance.Pop($"{_startChapter.ToString()}_Stage_{_startStage.ToString()}") as Stage;
            NextStage.GenerateStage(Vector3.zero);
        
            var player = PoolManager.Instance.Pop("Player") as PlayerController;
            NextStage.StageEnterEvent(player);
        }
    }        

    public override void UpdateManager()
    {
        // Do nothing

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            StageClear();
        }
    }

    public void GenerateNextStage(ChapterType chapter, int stage)
    {
        NextStage = PoolManager.Instance.Pop($"{chapter.ToString()}_Stage_{stage.ToString()}") as Stage;

        var dir = (CurrentStage.StageExitPoint - NextStage.StageEnterPoint).normalized;

        if (dir.x > dir.z)
        {
            dir = new Vector3(Mathf.Sign(dir.x), 0, 0);
        }
        else
        {
            dir = new Vector3(0, 0, Mathf.Sign(dir.z));
        }
        
        var exitPoint = CurrentStage.CenterPosition + CurrentStage.StageExitPoint;
        var enterPoint = exitPoint + (dir * _stageIntervalDistance);
        var nextStageCenter = enterPoint - 
            (new Vector3(NextStage.StageEnterPoint.x, 0, NextStage.StageEnterPoint.z).normalized * NextStage.StageEnterPoint.magnitude);
        
        GenerateBridge(exitPoint, enterPoint);
        NextStage.GenerateStage(nextStageCenter);
    }
    
    public void ChangeToNextStage(PlayerController player)
    {
        if (CurrentStage is not null)
        {
            CurrentStage.DisappearStage();
        }
        
        RemoveBridge();

        CurrentStage = NextStage;
        player.SetStage(CurrentStage);
        CurrentStage.InitStage();

        NextStage = null;
    }

    private void GenerateBridge(Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(BridgeGenerateRoutine(startPoint, endPoint, _bridgeGenerateDelay));
    }

    private void RemoveBridge()
    {
        StartCoroutine(BridgeRemoveRoutine(_bridgeGenerateDelay));
    }

    private IEnumerator BridgeGenerateRoutine(Vector3 startPoint, Vector3 endPoint, float delay)
    {
        var waitSecond = new WaitForSeconds(delay);
        
        var bridgeSize = _bridgeWidth + _bridgeIntervalDistance;
        var bridgeCount = (endPoint - startPoint).magnitude / bridgeSize;

        var bridgeDir = (endPoint - startPoint).normalized;
        var bridgeRotation = Quaternion.LookRotation(bridgeDir);

        for (var i = 1; i <= bridgeCount; i++)
        {
            var bridge = PoolManager.Instance.Pop("Bridge") as BridgeObject;
            var bridgePos = startPoint + (bridgeDir * (i * bridgeSize) - bridgeDir * (bridgeSize / 2f));
            
            bridge.SetWidth(_bridgeWidth);
            bridge.Generate(bridgePos, bridgeRotation);
            _bridgeObjects.Add(bridge);

            yield return waitSecond;
        }
    }
    
    private IEnumerator BridgeRemoveRoutine(float delay)
    {
        var waitSecond = new WaitForSeconds(delay);
        
        foreach (var bridge in _bridgeObjects)
        {
            bridge.Remove();
            yield return waitSecond;
        }
        
        _bridgeObjects.Clear();
    }

    public void StageClear()
    {
        CurrentStage.Converter.ConvertDimension(EAxisType.NONE);
        CurrentStage.SetActive(false);

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
