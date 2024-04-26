using UnityEngine;
using AxisConvertSystem;
using System;
using DG.Tweening;
using UnityEngine.InputSystem.Haptics;

public class Stage : Section
{
    [Header("Chapter Setting")]
    [SerializeField] private ChapterType _chapter;
    [SerializeField] private int _stageOrder;
    public int StageOrder => _stageOrder;

    public bool IsClear
    {
        get => _isClear;
        set
        {
            _isClear = value;
            if(value)
            {
                StoryManager.Instance.StartStoryIfCan(ChapterCondition.CHAPTER_CLEAR,_chapter,StageOrder);
            }
        }
    }
    private bool _isClear = false;

    public override void OnPop()
    {
        base.OnPop();
        IsClear = false;
    }

    protected override void FixedUpdate()
    {
        if (Active)
        {
            foreach (var unit in SectionUnits)
            {
                unit.FixedUpdateUnit();
            }
        }
    }

    protected override void Update()
    {
        if (Active)
        {
            foreach (var unit in SectionUnits)
            {
                unit.UpdateUnit();
            }
        }
    }

    public override void OnEnter(PlayerUnit player)
    {
        base.OnEnter(player);
        player.Converter.SetConvertable(true);

        if (StageManager.Instance.NextStage == this) 
        {
            StageManager.Instance.ChangeToNextStage();
        }
        StoryManager.Instance.StartStoryIfCan(ChapterCondition.CHAPTER_ENTER, _chapter, StageOrder);
    }

    public override void OnExit(PlayerUnit player)
    {
        base.OnExit(player);
        StoryManager.Instance.StartStoryIfCan(ChapterCondition.CHAPTER_EXIT,_chapter, StageOrder);
    }

    public override void Disappear(float dissolveTime = 1.5f, Action Callback = null)
    {
        ReloadSectionUnits();
        Dissolve(false, dissolveTime);
        transform.DOMove(CenterPosition - Vector3.up * SectionData.sectionYDepth, dissolveTime)
            .OnComplete(() =>
            {
                Active = false;
                Callback?.Invoke();
                SceneControlManager.Instance.SafeDeleteObject(this);
            });
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        gameObject.name = $"{_chapter.ToString().ToUpperFirstChar()}_Stage_{_stageOrder}";
    }
#endif
}
