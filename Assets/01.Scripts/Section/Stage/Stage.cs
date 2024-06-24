using UnityEngine;
using System;
using DG.Tweening;

public class Stage : Section
{
    [Header("Chapter Setting")]
    [SerializeField] private ChapterType _chapter;
    public ChapterType ChapterType => _chapter;
    [SerializeField] private int _stageOrder;
    public int StageOrder => _stageOrder;

    private const float ChangeGrayScaleDelay = 1f;
    
    public bool IsClear { get; set; } = false;

    public override void OnPop()
    {
        base.OnPop();
        IsClear = false;
    }

    public override void OnPush()
    {
        foreach (var unit in SectionUnits)
        {
            if (!unit.climbableUnit)
            {
                unit.DeleteClimbableEffect();
            }
            unit.OnPush();
        }
        base.OnPush();
    }

    public override void OnEnter(PlayerUnit player)
    {
        base.OnEnter(player);
        player.Converter.SetConvertable(true);

        if (StageManager.Instance.NextStage == this) 
        {
            StageManager.Instance.ChangeToNextStage();
        }
        StoryManager.Instance.StartStoryIfCan(StoryAppearType.STAGE_ENTER, _chapter, StageOrder);
    }

    public override void OnExit(PlayerUnit player)
    {
        base.OnExit(player);
        StoryManager.Instance.StartStoryIfCan(StoryAppearType.STAGE_EXIT,_chapter, StageOrder);
    }

    public override void Disappear(float dissolveTime = 1.5f, Action Callback = null)
    {
        ReloadSectionUnits();
        Dissolve(false, dissolveTime);
        transform.DOMove(CenterPosition - Vector3.up * SectionData.sectionYDepth, dissolveTime)
            .OnComplete(() =>
            {
                Active = false;
                SceneControlManager.Instance.SafeDeleteObject(this);
                Callback?.Invoke();
            });
    }
    
    public void StageClearFeedback()
    {
        foreach (var unit in SectionUnits)
        {
            if (unit is PlayerUnit)
            {
                continue;
            }
            
            unit.SetGrayScale(1f, ChangeGrayScaleDelay);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        gameObject.name = $"{_chapter.ToString().ToUpperFirstChar()}_Stage_{_stageOrder}";
    }
#endif
}
