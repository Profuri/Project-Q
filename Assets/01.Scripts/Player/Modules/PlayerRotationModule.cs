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
        rotateDir.Normalize();

        if (rotateDir == Vector3.zero)
            return;

        var rotateQuaternion = Quaternion.LookRotation(rotateDir);
        
        if (Controller.Converter.AxisType == EAxisType.NONE)
        {
            rotateQuaternion = Quaternion.Slerp(Controller.ModelTrm.rotation, rotateQuaternion, Controller.DataSO.rotationSpeed * Time.deltaTime);
        }
        
        Controller.ModelTrm.rotation = rotateQuaternion;
    }
}