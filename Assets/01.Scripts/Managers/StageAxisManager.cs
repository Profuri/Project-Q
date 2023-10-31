using ManagingSystem;
using UnityEngine;

public class StageAxisManager : BaseManager<StageAxisManager>
{
    private EAxisType _axisType;
    public EAxisType AxisType => _axisType;

    public override void Awake()
    {
        base.Awake();
        GameManager.Instance.OnAxisTypeChangeEvent += SetAxisType;
    }

    public override void StartManager()
    {
        SetAxisType(EAxisType.NONE);
    }

    public override void UpdateManager()
    {
        // Do Nothing
    }

    private void SetAxisType(EAxisType type)
    {
        if(_axisType != EAxisType.NONE)
            ReturnOriginalAxis();
        
        _axisType = type;
        
        if (_axisType != EAxisType.NONE)
            CompressAxis();
    }

    private void CompressAxis()
    {
        
    }

    private void ReturnOriginalAxis()
    {
        
    }
}