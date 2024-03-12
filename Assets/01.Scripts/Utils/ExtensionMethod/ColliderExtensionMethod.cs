using System.Linq;
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

    public static int NonAllocCast(this Collider col, out RaycastHit[] hits, AxisConverter converter)
    {
        hits = new RaycastHit[10];
        
        if (col is BoxCollider boxCol)
        {
            var boundsSize = boxCol.bounds.size * 0.9f;
            
            var center = boxCol.bounds.center;
            var dir = -Vector3ExtensionMethod.GetAxisDir(converter.AxisType);
            center -= dir;

            return Physics.BoxCastNonAlloc(center, boundsSize / 2f, dir, hits, col.transform.rotation, Mathf.Infinity);
        }
        
        if (col is CapsuleCollider capsuleCol)
        {   
            var radius = capsuleCol.radius * 0.7f;
            var height = capsuleCol.height * 0.7f;
            
            var center = capsuleCol.bounds.center;
            var dir = -Vector3ExtensionMethod.GetAxisDir(converter.AxisType);
            center -= dir;

            var p1 = center + Vector3.up * (height / 2f);
            var p2 = center - Vector3.up * (height / 2f);

            return Physics.CapsuleCastNonAlloc(p1, p2, radius, dir, hits, Mathf.Infinity);
        }

        return 0;
    }
}