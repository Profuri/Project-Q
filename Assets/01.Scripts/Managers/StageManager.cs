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
    public ChapterData CurrentPlayChapterData => _currentPlayChapterData;

    public event Action OnStageClear;

    public AxisType CurrentStageAxis => SceneControlManager.Instance.Player.Converter.AxisType;

    private StageRoadMapPanel _stageRoadMap;
    
    public override void StartManager()
    {
        CurrentStage = null;
        NextStage = null;
        _stageRoadMap = null;
        LoadToDataManager();
    }        

    public void StartNewChapter(ChapterData chapterData)
    {
        //IsClear = false;

        int stageIndex = DataManager.sSaveData.ChapterStageDictionary[chapterData.chapter];
        
        _currentPlayChapterData = chapterData;
        CurrentStage = SceneControlManager.Instance.AddObject(
            $"{chapterData.chapter.ToString().ToUpperFirstChar()}_Stage_{stageIndex}") as Stage;
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

                player.OnPop();
                
                player.SetSection(CurrentStage);
                player.useGravity = true;
                player.ReloadUnit(true, dissolveTime);
            });
        }
    }

    public void ShowRoadMap()
    {
        if (CurrentStage == null)
        {
            return;
        }
        
        if (_stageRoadMap != null && _stageRoadMap.poolOut)
        {
            _stageRoadMap.StopAllCoroutines();
            return;
        }
        
        _stageRoadMap = UIManager.Instance.GenerateUI("StageRoadMapPanel") as StageRoadMapPanel;
    }

    public void UnShowRoadMap()
    {
        if (CurrentStage == null)
        {
            return;
        }
        
        if (_stageRoadMap == null || !_stageRoadMap.poolOut)
        {
            return;
        }
        
        _stageRoadMap.Disappear();
    }
    
    public void ChangeToNextStage()
    {
        var lastStage = CurrentStage;
        CurrentStage = NextStage;
        CurrentStage.IsClear = false;
        DataManager.Instance.SaveData(this);
        
        if (lastStage is not null)
        {
            if (_stageRoadMap != null && _stageRoadMap.poolOut)
            {
                _stageRoadMap.SetUnitEnable(lastStage.StageOrder, false);
                _stageRoadMap.SetUnitEnable(lastStage.StageOrder + 1, true);
            }
            else
            {
                _stageRoadMap = UIManager.Instance.GenerateUI("StageRoadMapPanel", null, component =>
                {
                    (component as StageRoadMapPanel)?.SetUnitEnable(lastStage.StageOrder, false);
                    (component as StageRoadMapPanel)?.SetUnitEnable(lastStage.StageOrder + 1, true);
                    (component as StageRoadMapPanel)?.AutoDisappear();
                }) as StageRoadMapPanel;
                _stageRoadMap.SetUnitEnable(lastStage.StageOrder, true, 0f);
                _stageRoadMap.SetUnitEnable(lastStage.StageOrder + 1, false, 0f);
            }
            
            lastStage.Disappear();
            lastStage.RemoveBridge();
        }
    }

    public void StageClear(PlayerUnit player)
    {
        if (CurrentStage is null || IsClear)
        {
            return;
        }
        
        var nextStageIndex = CurrentStage.StageOrder + 1;
        OnStageClear?.Invoke();
        CurrentStage.IsClear = true;
        
        DataManager.Instance.SaveData(this);
        
        CurrentStage.Lock = false;
        player.OriginUnitInfo.LocalPos = CurrentStage.PlayerResetPointInClear;

        if (player.Converter.AxisType != AxisType.None)
        {
            player.Converter.ConvertDimension(AxisType.None, StageClearFeedback);
        }
        else
        {
            StageClearFeedback();
        }
        
        player.Converter.SetConvertable(false);

        if (nextStageIndex >= _currentPlayChapterData.stageCnt)
        {
            return;
        }
                
        GenerateNextStage(_currentPlayChapterData.chapter, nextStageIndex);
    }

    private void StageClearFeedback()
    {
        if (CurrentStage.ChapterType == ChapterType.Tutorial)
        {
            SceneControlManager.Instance.LoadScene(SceneType.Chapter);
            return;
        }
        
        CurrentStage.StageClearFeedback(() =>
        {
            var nextStageIndex = CurrentStage.StageOrder + 1;

            if (nextStageIndex < _currentPlayChapterData.stageCnt)
            {
                return;
            }

            SceneControlManager.Instance.LoadScene(SceneType.Chapter, null, () =>
            {
                if (CurrentStage.ChapterType == ChapterType.Cpu)
                {
                    var thanksToPanel = UIManager.Instance.GenerateUI("ThanksToPanel");
                    thanksToPanel.ResetPosition();
                }
            }, () =>
            {
                if (CurrentStage.ChapterType == ChapterType.Cpu)
                {
                    InputManager.Instance.SetEnableInputAll(false);
                }
            });
        });
    }
    
    public void ReleaseChapter()
    {
        _currentPlayChapterData = null;
    }

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            if (_currentPlayChapterData == null) 
                return;
            
            var currentChapter = _currentPlayChapterData.chapter;
            var isClearChapter = CurrentStage.IsClear && CurrentStage.StageOrder + 1 >= _currentPlayChapterData.stageCnt;
            
            saveData.ChapterClearDictionary[currentChapter] = isClearChapter;
            saveData.ChapterStageDictionary[currentChapter] = CurrentStage.StageOrder;
        };
    }

    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(this,null);
    }
}
