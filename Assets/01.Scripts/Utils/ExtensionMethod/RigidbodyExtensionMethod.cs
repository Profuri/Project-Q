using AxisConvertSystem;
using UnityEngine;

public static class RigidbodyExtensionMethod
{
    public static void FreezeAxisPosition(this Rigidbody rigid, AxisType axis, bool clearConstraints = true)
    {
        if (clearConstraints)
        {
            rigid.constraints = RigidbodyConstraints.FreezeRotation;
        }
        
        if (axis == AxisType.X)
        {
            rigid.constraints |= RigidbodyConstraints.FreezePositionX;
        }
        else if (axis == AxisType.Y)
        {
            rigid.constraints |= RigidbodyConstraints.FreezePositionY;
        }
        else if (axis == AxisType.Z)
        {
            rigid.constraints |= RigidbodyConstraints.FreezePositionZ;
        }
    }
}