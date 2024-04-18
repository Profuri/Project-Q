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
    public int stageOrder => _stageOrder;

    public bool IsClear { get; set; }
    public bool IsConverting => !SceneControlManager.Instance.Player.Converter.Convertable;

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
                if(!IsConverting || IsClear)
                {
                    if(unit.TryGetComponent(out IUpdateBlockable blockable))
                    {
                        //do thing   
                        continue;
                    }
                }
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
                if(!IsConverting || IsClear)
                {
                    if(unit.TryGetComponent(out IUpdateBlockable blockable))
                    {
                        //do thing   
                        continue;
                    }
                }
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
