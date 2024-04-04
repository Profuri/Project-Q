using AxisConvertSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBHodlingUnit : RGBObjectUnit
{
    public override void UpdateUnit()
    {
        base.UpdateUnit();
    }
    public override void Convert(AxisType axis)
    {
         base.Convert(axis);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        base.OnInteraction(communicator, interactValue, param);


    }
}
