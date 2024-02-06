using Unity.Mathematics;
using UnityEngine;

public static class TransformExtensionMethod
{
    public static void Reset(this Transform trm)
    {
        trm.localPosition = Vector3.zero;
        trm.localRotation = quaternion.identity;
        trm.localScale = Vector3.one;
    }
}
