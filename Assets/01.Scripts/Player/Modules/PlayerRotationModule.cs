using AxisConvertSystem;
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
        var rotateDir = Controller.GetModule<PlayerMovementModule>().MoveVelocity;
        rotateDir.y = 0;
        rotateDir.Normalize();

        if (rotateDir == Vector3.zero)
            return;

        var rotateQuaternion = Quaternion.LookRotation(rotateDir);
        
        if (Controller.Converter.AxisType == AxisType.None)
        {
            rotateQuaternion = Quaternion.Slerp(Controller.ModelTrm.rotation, rotateQuaternion, Controller.Data.rotationSpeed * Time.deltaTime);
        }
        
        Controller.ModelTrm.rotation = rotateQuaternion;
    }
}