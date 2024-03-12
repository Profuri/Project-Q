using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;

public static class Vector3ExtensionMethod
{
    public static Vector3 Multiply(this Vector3 vector, Vector3 other)
    {
        return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
    }

    public static void SetAxisElement(this ref Vector3 vector, AxisType axis, float value)
    {
        if (axis == AxisType.X)
        {
            vector.x = value;
        }
        else if (axis == AxisType.Y)
        {
            vector.y = value;
        }
        else if (axis == AxisType.Z)
        {
            vector.z = value;
        }
    }

    public static float GetAxisElement(this Vector3 vector, AxisType axis)
    {
        if (axis == AxisType.X)
        {
            return vector.x;
        }
        else if (axis == AxisType.Y)
        {
            return vector.y;
        }
        else if (axis == AxisType.Z)
        {
            return vector.z;
        }

        return 0f;
    }
    
    public static Vector3 GetAxisDir(AxisType axis)
    {
        return new Vector3(axis == AxisType.X ? 1 : 0, axis == AxisType.Y ? 1 : 0, axis == AxisType.Z ? -1 : 0);
    }
}
