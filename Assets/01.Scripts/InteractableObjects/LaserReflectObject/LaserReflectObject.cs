using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class LaserReflectObject : InteractableObject
{
    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        var laserObject = (LaserLauncherObject)communicator;
        
        var point = (Vector3)param[0];
        var normal = (Vector3)param[1];
        var lastDir = (Vector3)param[2];

        var dir = Vector3.Reflect(lastDir, normal).normalized;
        
        laserObject.AddLaser(new LaserInfo { origin = point, dir = dir });
    }
}