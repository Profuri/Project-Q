using UnityEngine;

public class PlayerIdleState : PlayerOnGroundState
{
    private bool _isRotationPlayer;
    
    public PlayerIdleState(StateController controller, bool useAnim = false, string animationKey = "") : base(controller, useAnim, animationKey)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        InputManager.Instance.PlayerInputReader.OnPlayerRotateEnterEvent += RotatingEnterHandle;
        InputManager.Instance.PlayerInputReader.OnPlayerRotateExitEvent += RotatingExitHandle;
        
        Player.CanAxisControl = true;
        Player.StopImmediately(false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        PlayerRotating();

        Player.StopImmediately(false);

        var movementInput = InputManager.Instance.PlayerInputReader.movementInput;
        if (movementInput.sqrMagnitude > 0.05f)
        {
            Controller.ChangeState(typeof(PlayerMovementState));
        }
    }

    public override void ExitState()
    {
        InputManager.Instance.PlayerInputReader.OnPlayerRotateEnterEvent -= RotatingEnterHandle;
        InputManager.Instance.PlayerInputReader.OnPlayerRotateExitEvent -= RotatingExitHandle;
        
        base.ExitState();
    }

    private void RotatingEnterHandle()
    {
        _isRotationPlayer = true;
    }
    
    private void RotatingExitHandle()
    {
        _isRotationPlayer = false;
    }

    private void PlayerRotating()
    {
        if (!_isRotationPlayer)
        {
            return;
        }

        var currentRot = Player.ModelTrm.rotation;
        var mouseDelta = InputManager.Instance.PlayerInputReader.mouseDelta.x;
        if (mouseDelta == 0f)
        {
            return;
        }

        var mouseDeltaDir = Mathf.Sign(mouseDelta);
        var nextRot = currentRot * Quaternion.Euler(Vector3.up * (mouseDeltaDir * 360f * Time.deltaTime));
        Player.Rotate(nextRot, 1f);
    }
}
