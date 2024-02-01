using System.Collections.Generic;
using AxisConvertSystem;

public class PictureObject : StructureObjectUnitBase
{
    private List<PictureUnit> _units;

    public override void Init(AxisConverter converter)
    {
        _units = new List<PictureUnit>();
        transform.GetComponentsInChildren(_units);
        foreach (var unit in _units)
        {
            unit.Init();
        }
        base.Init(converter);
    }

    public override void ObjectSetting()
    {
        UnitSetting(_objectInfo.axis);
        base.ObjectSetting();
    }

    private void UnitSetting(AxisType axis)
    {
        foreach (var unit in _units)
        {
            unit.ChangeAxis(axis);
        }
    }

    protected override void ColliderSetting()
    {
        base.ColliderSetting();

        if (_objectInfo.axis != AxisType.Y && _objectInfo.axis != AxisType.None)
        {
            _collider.isTrigger = true;
        }
    }
}