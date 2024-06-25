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
        base.Appear(parentTrm, component =>
        {
            callback?.Invoke(component);
            StartCoroutine(LifeCycleRoutine());
        });
        
        var stageInfos = StageManager.Instance.CurrentPlayChapterData.stageInfos;
        
        // 형주가 스테이지 저장 만들어주면 합치기
        // var stageClearData = DataManager.sSaveData.
        
        for (var i = 0; i < stageInfos.Count; i++)
        {
            var unit = UIManager.Instance.GenerateUI("StageRoadMapUnit", _unitParent) as StageRoadMapUnit;
            unit.SetUp(stageInfos[i], false);
            unit.SetEnable(false, 0f);
            _units.Add(unit);
        }
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
    }

    public void SetUnitEnable(int index, bool enable)
    {
        if (index < 0 || index >= _units.Count)
        {
            return;
        }
        
        _units[index].SetEnable(enable);
    }

    private IEnumerator LifeCycleRoutine()
    {
        yield return _lifeWaitTime;
        Disappear();
    }
}
