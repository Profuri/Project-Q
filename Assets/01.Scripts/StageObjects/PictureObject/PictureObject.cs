using System.Collections.Generic;
using StageStructureConvertSystem;

public class PictureObject : StructureObjectUnitBase
{
    private List<PictureUnit> _units;

    public override void Init(StructureConverter converter)
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

    private void UnitSetting(EAxisType axis)
    {
        foreach (var unit in _units)
        {
            unit.ChangeAxis(axis);
        }
    }

    protected override void ColliderSetting()
    {
        base.ColliderSetting();

        if (_objectInfo.axis != EAxisType.Y && _objectInfo.axis != EAxisType.NONE)
        {
            _collider.isTrigger = true;
        }
    }
}