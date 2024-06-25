using System;
using System.Collections.Generic;
using UnityEngine;

public class StageRoadMapPanel : UIComponent
{
    private Transform _unitParent;
    private List<StageRoadMapUnit> _units;

    private int _currentAccessUnitIndex;

    protected override void Awake()
    {
        base.Awake();
        _unitParent = transform.Find("UnitParent");
        _units = new List<StageRoadMapUnit>();
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, callback);
        
        _currentAccessUnitIndex = -1;

        var stageInfos = StageManager.Instance.CurrentPlayChapterData.stageInfos;
        
        // 형주가 스테이지 저장 만들어주면 합치기
        // var stageClearData = DataManager.sSaveData.

        for (var i = 0; i < stageInfos.Count; i++)
        {
            var unit = UIManager.Instance.GenerateUI("StageRoadMapUnit", _unitParent) as StageRoadMapUnit;
            unit.ResetPosition();
            unit.SetUp(stageInfos[i], false);
            _units.Add(unit);
        }
    }

    public override void Disappear(Action callback = null)
    {
        base.Disappear(() =>
        {
            foreach (var unit in _units)
            {
                unit.Disappear();   
            }
            callback?.Invoke();
        });
    }

    public void SetUnitEnable(int newIndex)
    {
        if (_currentAccessUnitIndex != -1)
        {
            _units[_currentAccessUnitIndex].SetEnable(false);
        }

        _currentAccessUnitIndex = newIndex;
        _units[_currentAccessUnitIndex].SetEnable(true);
    }
}