using UnityEngine;

public static class Vector3ExtensionMethod
{
    public static Vector3 Multiply(this Vector3 vector, Vector3 other)
    {
        return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
    }
}