using System.Collections.Generic;
using AxisConvertSystem;

public class PictureObject : ObjectUnit
{
    private List<PictureUnit> _units;

    public override void Awake()
    {
        base.Awake();
        _units = new List<PictureUnit>();
        transform.GetComponentsInChildren(_units);
        foreach (var unit in _units)
        {
            unit.Init();
        }
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);
        foreach (var unit in _units)
        {
            unit.ChangeAxis(axis);
        }
        if (axis != AxisType.Y && axis != AxisType.None)
        {
            Collider.isTrigger = true;
        }
    }
}