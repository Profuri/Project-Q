using AxisConvertSystem;
using UnityEngine;

public static class Vector3ExtensionMethod
{
    public static Vector3 Multiply(this Vector3 vector, Vector3 other)
    {
        return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
    }

    public static Vector3 GetAxisDir(AxisType axis)
    {
        return new Vector3(axis == AxisType.X ? -1 : 0, axis == AxisType.Y ? -1 : 0, axis == AxisType.Z ? 1 : 0);
    }
}