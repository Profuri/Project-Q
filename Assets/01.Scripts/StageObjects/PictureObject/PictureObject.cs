using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;

public class PictureObject : ObjectUnit
{
    [SerializeField] private Material _enableMat;
    [SerializeField] private Material _disableMat;

    private List<PictureUnit> _units;

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

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);
        foreach (var unit in _units)
        {
            unit.ChangeAxis(axis);
        }

        var prev = Collider.isTrigger;
        Collider.isTrigger = axis != AxisType.Y && axis != AxisType.None;
        if (prev != Collider.isTrigger)
        {
            Dissolve(Collider.isTrigger ? 0.5f : 0f, 0.5f, false);
        }
    }
}