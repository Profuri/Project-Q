using AxisConvertSystem;
using UnityEngine;

public class PlaneUnit : ObjectUnit
{
    protected override UnitInfo ConvertInfo(UnitInfo basic, AxisType axis)
    {
        var info = base.ConvertInfo(basic, axis);
        info.ColliderCenter = Vector3.zero;
        return info;
    }
}