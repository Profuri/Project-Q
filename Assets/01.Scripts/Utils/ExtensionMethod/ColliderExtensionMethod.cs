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
        else if (col is CharacterController characterController)
        {
            characterController.center = center;
        }
    }
}