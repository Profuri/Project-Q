using AxisConvertSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBHodlingUnit : RGBObjectUnit, IHoldable
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (communicator is PlayerUnit playerUnit)
        {
            if(MatchRGBColor)
            {
                return;
            }
            playerUnit.HoldingHandler.Attach(this);
        }
        else
        {
            base.OnInteraction(communicator, interactValue, param);
        }
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);
    }
    public ObjectUnit GetObjectUnit() => this;
}
