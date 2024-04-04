using AxisConvertSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBHodlingUnit : RGBObjectUnit, IHoldable
{
    public override void Convert(AxisType axis)
    {
         base.Convert(axis);
    }


    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (communicator is PlayerUnit playerUnit)
        {
            playerUnit.HoldingHandler.Attach(this);
        }
        else
        {
            base.OnInteraction(communicator, interactValue, param);
        }
    }

    public ObjectUnit GetObjectUnit() => this;
}
