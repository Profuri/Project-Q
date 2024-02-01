using System;
using AxisConvertSystem;
using InputControl;
using ModuleSystem;
using UnityEngine;
using static Core.Define;

public class PlayerMovementModule : BaseModule<PlayerController>
{
    [SerializeField] private InputReader _inputReader;
    
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _maxGroundCheckDistance;

    private Vector3 _inputDir;

    private Vector3 _force;
    private Vector3 _moveVelocity;

    private bool _canMove = true;

    public Vector3 MoveVelocity => _moveVelocity;

    public bool CanJump { get; set; }
    public bool IsMovement => _inputDir.sqrMagnitude > 0f;
    public bool IsGround { get; private set; }

    public override void Init(Transform root)
    {
        base.Init(root);

        CanJump = true;

        _inputReader.OnMovementEvent += SetInputDir;
        _inputReader.OnJumpEvent += OnJump;
    }

    public override void UpdateModule()
    {
        if(_canMove)
        {
            IsGround = CheckGround();
            Controller.PlayerAnimatorController.IsGround(IsGround);
            Controller.PlayerAnimatorController.Movement(_inputDir.sqrMagnitude >= 0.05f);
            CalcMovement();
        }
    }

    public override void FixedUpdateModule()
    {
        if (_moveVelocity.sqrMagnitude >= 0.05f)
        {
            Controller.CharController.Move(_moveVelocity * Time.deltaTime);
        }
    }

    public override void DisableModule()
    {
        base.DisableModule();
        
        _inputReader.OnMovementEvent -= SetInputDir;
        _inputReader.OnJumpEvent -= OnJump;
    }

    public void SetEnableMove(bool value)
    {
        StopImmediately();
        _canMove = value;
    }

    private void CalcMovement()
    {
        switch (Controller.Converter.AxisType)
        {
            case AxisType.None:
                _moveVelocity = (_inputDir.x * MainCam.RightView() + _inputDir.z * MainCam.ForwardView()) * Controller.DataSO.walkSpeed;
                break;
            case AxisType.Y:
                _moveVelocity = (_inputDir.x * MainCam.RightView() + _inputDir.z * MainCam.UpView()) * Controller.DataSO.walkSpeed;
                break;
            case AxisType.X:
                _moveVelocity = new Vector3(0, 0, _inputDir.x) * Controller.DataSO.walkSpeed;
                break;
            case AxisType.Z:
                _moveVelocity = new Vector3(_inputDir.x, 0, 0) * Controller.DataSO.walkSpeed;
                break;
        }

        _moveVelocity += _force;
        
        if(IsGround && _force.y < 0f)
        {
            SetVerticalVelocity(-1f);
        }
        else
        {
            AddVerticalVelocity(Controller.DataSO.gravity * Time.deltaTime);
        }

        _force = new Vector3(0, _force.y, 0);
    }

    private void SetVerticalVelocity(float value)
    {
        _force.y = value;
    }

    private void AddVerticalVelocity(float value)
    {
        _force.y += value;
    }

    public void SetForce(Vector3 force)
    {
        _force = force;
    }

    public void AddForce(Vector3 force)
    {
        _force += force;
    }

    private bool CheckGround()
    {
        var size = Controller.CharController.bounds.size * 0.8f;
        size.y = 0.1f;
        var isHit = Physics.BoxCast(
            Controller.CenterPoint.position,
            size,
            -transform.up,
            out var hit,
            Controller.ModelTrm.rotation,
            _maxGroundCheckDistance,
            _groundMask
        );
        
        return isHit && !hit.collider.isTrigger;
    }

    public void StopImmediately()
    {
        _force = new Vector3(0, -1f, 0);
        _moveVelocity = Vector3.zero;
    }

    private void SetInputDir(Vector2 input)
    {
        _inputDir = new Vector3(input.x, 0, input.y);
    }
    
    private void OnJump()
    {
        if (IsGround && CanJump)
        {
            SetVerticalVelocity(Controller.DataSO.jumpPower);
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (Controller == null || Controller.CharController == null)
        {
            return;
        }

        var size = Controller.CharController.bounds.size * 0.8f;
        size.y = 0.1f;
        var trm = transform;
        Gizmos.DrawCube(Controller.CenterPoint.position - trm.up * _maxGroundCheckDistance, size);
    }
#endif
}
