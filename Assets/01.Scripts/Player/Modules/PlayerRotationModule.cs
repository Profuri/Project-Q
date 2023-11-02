using ModuleSystem;
using UnityEngine;

public class PlayerRotationModule : BaseModule<PlayerController>
{
    public override void UpdateModule()
    {
        Rotate();
    }

    public override void FixedUpdateModule()
    {
        // Do Nothing
    }

    private void Rotate()
    {
        Vector3 rotateDir = Controller.GetModule<PlayerMovementModule>().MoveVelocity;
        rotateDir.y = 0;

        if (Controller.Converter.AxisType == EAxisType.X)
            rotateDir.x = 0;
        if (Controller.Converter.AxisType == EAxisType.Z)
            rotateDir.z = 0;
        
        rotateDir.Normalize();

        if (rotateDir != Vector3.zero)
        {
            var rotateQuaternion = Quaternion.LookRotation(rotateDir);
            var lerpQuaternion = Quaternion.Slerp(Controller.ModelTrm.rotation, rotateQuaternion, Controller.DataSO.rotationSpeed * Time.deltaTime);
            Controller.ModelTrm.rotation = lerpQuaternion;
        }
    }
}