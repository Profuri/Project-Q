using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class LaserReflectObject : InteractableObject
{
    [SerializeField] private float _rotateSpeed;
    
    public override void Awake()
    {
        base.Awake();
    }
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if(communicator is PlayerUnit)
        {
            RotateTransform();
            return;
        }

        var laserObject = (LaserLauncherObject)communicator;

        var point   = (Vector3)param[0];
        var normal  = (Vector3)param[1];
        var lastDir = (Vector3)param[2];

        var dir = Vector3.Reflect(lastDir, normal).normalized;
        
        laserObject.AddLaser(new LaserInfo { origin = point, dir = dir });
    }

    private Quaternion RotateTransform()
    {
        transform.Rotate(transform.up * _rotateSpeed * Time.deltaTime);
        return this.transform.rotation;
    }
}