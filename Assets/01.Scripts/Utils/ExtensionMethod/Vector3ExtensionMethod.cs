using System.Collections.Generic;
using AxisConvertSystem;
using UnityEngine;

public static class Vector3ExtensionMethod
{
    public static Vector3 Multiply(this Vector3 vector, Vector3 other)
    {
        return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
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

    public static List<Vector3> CalcAxisBounds(AxisType axis, Bounds bound)
    {
        return CalcAxisBounds(axis, bound.max, bound.min);
    }
    
    public static List<Vector3> CalcAxisBounds(AxisType axis, Vector3 max, Vector3 min)
    {
        var list = new List<Vector3>();

        if (axis == AxisType.X)
        {
            list.Add(max);
            list.Add(new Vector3(max.x, min.y, max.z));
            list.Add(new Vector3(max.x, min.y, min.z));
            list.Add(new Vector3(max.x, max.y, min.z));
        }
        else if (axis == AxisType.Y)
        {
            list.Add(new Vector3(max.x, max.y, max.z));
            list.Add(new Vector3(max.x, max.y, min.z));
            list.Add(new Vector3(min.x, max.y, min.z));
            list.Add(new Vector3(min.x, max.y, max.z));
        }
        else if (axis == AxisType.Z)
        {
            list.Add(new Vector3(max.x, max.y, min.z));
            list.Add(new Vector3(max.x, min.y, min.z));
            list.Add(min);
            list.Add(new Vector3(min.x, max.y, min.z));
        }
        
        return list;
    }
}