using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;

public class PictureObject : ObjectUnit
{
    [SerializeField] private Material _enableMat;
    [SerializeField] private Material _disableMat;

    private List<PictureUnit> _units;

    private bool _enable;

    public override void Awake()
    {
        base.Awake();
        _units = new List<PictureUnit>();
        transform.GetComponentsInChildren(_units);
        foreach (var unit in _units)
        {
            unit.Init(_enableMat, _disableMat);
        }
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _enable = true;
    }

    public override void Convert(AxisType axis)
    {
        foreach (var unit in _units)
        {
            unit.ChangeAxis(axis);
        }

        base.Convert(axis);
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);

        if (axis == AxisType.Y)
        {
            return;
        }

        var enable = axis == AxisType.None;
        
        if (_enable != enable)
        {
            Collider.isTrigger = !enable;
            Dissolve(Collider.isTrigger ? 0.55f : 0f, 0.5f, false);
            _enable = enable;
        }
    }
}