using System;
using InputControl;
using ModuleSystem;
using UnityEngine;
using static Core.Define;

public class PlayerMovementModule : BaseModule<PlayerController>
{
    [SerializeField] private InputReader _inputReader;
    
    private CharacterController _characterController;

    private Vector3 _inputDir;
    
    private Vector3 _moveVelocity;
    private Vector3 _verticalVelocity;

    public Vector3 MoveVelocity => _moveVelocity;
    public bool IsGround => _characterController.isGrounded;

    public override void Init(Transform root)
    {
        base.Init(root);

        _characterController = root.GetComponent<CharacterController>();

        _inputReader.OnMovementEvent += SetInputDir;
        _inputReader.OnJumpEvent += OnJump;
    }

    public override void UpdateModule()
    {
        CalcMovement();
    }

    public override void FixedUpdateModule()
    {
        _characterController.Move(_moveVelocity * Time.deltaTime);
    }

    public override void DisableModule()
    {
        base.DisableModule();
        
        _inputReader.OnMovementEvent -= SetInputDir;
        _inputReader.OnJumpEvent -= OnJump;
    }

    private void CalcMovement()
    {
        switch (StageAxisManager.Instance.AxisType)
        {
            case EAxisType.NONE:
                _moveVelocity = (_inputDir.x * MainCam.RightView() + _inputDir.z * MainCam.ForwardView()) * Controller.DataSO.walkSpeed;
                break;
            case EAxisType.EXPRESSION_Y:
                _moveVelocity = (_inputDir.x * MainCam.RightView() + _inputDir.z * MainCam.UpView()) * Controller.DataSO.walkSpeed;
                break;
            case EAxisType.EXPRESSION_X:
                _moveVelocity = new Vector3(0, 0, _inputDir.x) * Controller.DataSO.walkSpeed;
                break;
            case EAxisType.EXPRESSION_Z:
                _moveVelocity = new Vector3(_inputDir.x, 0, 0) * Controller.DataSO.walkSpeed;
                break;
        }
        
        _moveVelocity += _verticalVelocity;

        if (!IsGround)
        {
            _verticalVelocity.y += Controller.DataSO.gravity * Time.deltaTime;
        }
    }

    private void SetInputDir(Vector2 input)
    {
        _inputDir = new Vector3(input.x, 0, input.y);
    }
    
    private void OnJump()
    {
        if(IsGround)
            _verticalVelocity.y = Controller.DataSO.jumpPower;
    }
}
