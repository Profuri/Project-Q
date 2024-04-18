 using AxisConvertSystem;
using UnityEngine;

public class PlaneUnit : ObjectUnit
{
    public override void Awake()
    {
        base.Awake();
        compressLayer = CompressLayer.Plane;
    }

    protected override UnitInfo ConvertInfo(UnitInfo basic, AxisType axis)
    {
        var info = base.ConvertInfo(basic, axis);
        info.LocalScale = UnitInfo.LocalScale;
        info.ColliderCenter = Vector3.zero;
        return info;
    }
}