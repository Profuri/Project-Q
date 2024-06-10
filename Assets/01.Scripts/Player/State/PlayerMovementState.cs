using AxisConvertSystem;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementState : PlayerOnGroundState
{
    public PlayerMovementState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }
    
    public override void UpdateState()
    {
        base.UpdateState();

        var movementInput = InputManager.Instance.PlayerInputReader.movementInput;
        var dir = Quaternion.Euler(0, CameraManager.Instance.ActiveVCam.transform.eulerAngles.y, 0) * movementInput;
        if (Player.Converter.AxisType != AxisType.None)
        {
            dir.SetAxisElement(Player.Converter.AxisType, 0);
        }
        
        if (dir.sqrMagnitude < 0.05f)
        {
            Controller.ChangeState(typeof(PlayerIdleState));
            return;
        }



        Player.Rotate(Quaternion.LookRotation(dir), Player.Converter.AxisType is AxisType.None or AxisType.Y ? Player.Data.rotationSpeed : 1f);
        Player.SetVelocity(dir * Player.Data.walkSpeed);

        RaycastHit hit;

        if (Player.IsStairOnNextStep(out hit))
        {
            //var originPos = Player.transform.position;
            //originPos.y = hit.collider.bounds.max.y;
            //Player.transform.position = originPos;
            Vector3 velocity = Player.Rigidbody.velocity;
            velocity.y += 0.15f;
            Player.SetVelocity(velocity);
            Debug.Log("true");
        }

    }


}