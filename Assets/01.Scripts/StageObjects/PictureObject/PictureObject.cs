using System.Collections.Generic;
using AxisConvertSystem;

public class PictureObject : ObjectUnit
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

    public void ObjectSetting()
    {
        // UnitSetting(_objectInfo.CompressType);
        // base.ObjectSetting();
    }

    private void UnitSetting(AxisType axis)
    {
        foreach (var unit in _units)
        {
            unit.ChangeAxis(axis);
        }
    }

    protected void ColliderSetting()
    {
        // base.ColliderSetting();

        // if (_objectInfo.CompressType != AxisType.Y && _objectInfo.CompressType != AxisType.None)
        // {
            // _collider.isTrigger = true;
        // }
    }
}