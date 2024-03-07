using AxisConvertSystem;
using UnityEngine;

public static class ColliderExtensionMethod
{
    public static void SetCenter(this Collider col, Vector3 center)
    {
        if (col is BoxCollider boxCol)
        {
            boxCol.center = center;
        }
        else if (col is CapsuleCollider capsuleCol)
        {
            capsuleCol.center = center;
        }
    }

    public static Vector3 GetLocalCenter(this Collider col)
    {
        if (col is BoxCollider boxCol)
        {
            return boxCol.center;
        }
        
        if (col is CapsuleCollider capsuleCol)
        {
            return capsuleCol.center;
        }
        
        return Vector3.zero;
    }

    public static DepthPoint GetDepthPoint(this Collider col, AxisType axis)
    {
        var depthPoint = new DepthPoint();

        var max = col.bounds.max;
        var min = col.bounds.min;
        var bounds = col.bounds;

        if (axis == AxisType.X)
        {
            depthPoint.Max = new Vector2(max.z, max.y); // RT
            depthPoint.Min = new Vector2(min.z, min.y); // LB
            depthPoint.Z = max.x;
        }

        else if (axis == AxisType.Y)
        {
            depthPoint.Max = new Vector2(max.x, max.z); // RT
            depthPoint.Min = new Vector2(min.x, min.z); // LB
            depthPoint.Z = max.y;
        }

        else if (axis == AxisType.Z)
        {
            depthPoint.Max = new Vector2(max.x, max.y); // RT
            depthPoint.Min = new Vector2(min.x, min.y); // LB
            depthPoint.Z = -min.z;
            //depthPoint.Z = max.
        }

        depthPoint.IsTrigger = col.isTrigger;
        
        return depthPoint;
    }
}