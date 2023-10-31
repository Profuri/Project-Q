using ManagingSystem;

public class AxisManager : BaseManager<AxisManager>
{
    private EAxisType _axisType;
    public EAxisType AxisType => _axisType;
    
    public override void StartManager()
    {
        SetAxisType(EAxisType.NONE);
    }

    public override void UpdateManager()
    {
        // Do Nothing
    }

    public void SetAxisType(EAxisType type)
    {
        if(_axisType != EAxisType.NONE)
            ReturnOriginalAxis();
        
        _axisType = type;
        
        if (_axisType != EAxisType.NONE)
            CompressAxis(_axisType);
    }

    private void CompressAxis(EAxisType type)
    {
        
    }

    private void ReturnOriginalAxis()
    {
        
    }
}