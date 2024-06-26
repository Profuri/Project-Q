using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRoadMapPanel : UIComponent
{
    [SerializeField] private float _lifeTime = 2f;
    
    private Transform _unitParent;
    private List<StageRoadMapUnit> _units;
    
    private WaitForSeconds _lifeWaitTime;

    protected override void Awake()
    {
        base.Awake();
        _lifeWaitTime = new WaitForSeconds(_lifeTime);
        _unitParent = transform.Find("UnitParent");
        _units = new List<StageRoadMapUnit>();
    }

    public override void Appear(Transform parentTrm, Action<UIComponent> callback = null)
    {
        var curChapterData = StageManager.Instance.CurrentPlayChapterData;
        var stageInfos = curChapterData.stageInfos;

        for (var i = 0; i < stageInfos.Count; i++)
        {
            var unit = UIManager.Instance.GenerateUI("StageRoadMapUnit", _unitParent) as StageRoadMapUnit;
            unit.SetUp(stageInfos[i], DataManager.sSaveData.ChapterStageDictionary[curChapterData.chapter] > i);
            unit.SetEnable(false, 0f);
            _units.Add(unit);
        }
        
        _units[StageManager.Instance.CurrentStage.StageOrder].SetEnable(true, 0f);

        base.Appear(parentTrm, callback);
    }

    public override void Disappear(Action<UIComponent> callback = null)
    {
        base.Disappear(component =>
        {
            foreach (var unit in _units)
            {
                unit.Disappear();   
            }
            _units.Clear();
            callback?.Invoke(component);
        });
        
        foreach (var unit in _units)
        {
            if (!unit.Enable)
            {
                continue;
            }
            
            unit.SetEnable(false);   
        }
    }

    public void SetUnitEnable(int index, bool enable, float time = -1f)
    {
        if (index < 0 || index >= _units.Count)
        {
            return;
        }
        
        _units[index].SetEnable(enable, time);
    }

    public void AutoDisappear()
    {
        StartCoroutine(LifeCycleRoutine());
    }

    private IEnumerator LifeCycleRoutine()
    {
        yield return _lifeWaitTime;
        Disappear();
    }
}
