using UnityEngine;

public static class CameraExtensionMethod
{
    public static Vector3 ForwardView(this Camera camera)
    {
        return new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z).normalized;
    }

    public static Vector3 UpView(this Camera camera)
    {
        return new Vector3(camera.transform.up.x, 0, camera.transform.up.z).normalized;
    }

    public static Vector3 RightView(this Camera camera)
    {
        return new Vector3(camera.transform.right.x, 0, camera.transform.right.z).normalized;
    }
}