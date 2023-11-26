using System;
using InputControl;
using ModuleSystem;
using UnityEngine;
using static Core.Define;

public class PlayerMovementModule : BaseModule<PlayerController>
{
    [SerializeField] private InputReader _inputReader;
    
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _maxGroundCheckDistance;

    private BoxCollider _collider;
    private Rigidbody _rigidbody;

    private Vector3 _inputDir;
    
    private Vector3 _moveVelocity;
    private Vector3 _verticalVelocity;

    private bool _canMove = true;

    public Vector3 MoveVelocity => _moveVelocity;

    public bool CanJump { get; set; }
    public bool IsGround => CheckGround();

    public override void Init(Transform root)
    {
        base.Init(root);

        _collider = root.GetComponent<BoxCollider>();
        _rigidbody = root.GetComponent<Rigidbody>();

        _inputReader.OnMovementEvent += SetInputDir;
        _inputReader.OnJumpEvent += OnJump;
    }

    public override void UpdateModule()
    {
        if(_canMove)
        {
            CalcMovement();
        }
    }

    public override void FixedUpdateModule()
    {
        _rigidbody.velocity = _moveVelocity;
    }

    public override void DisableModule()
    {
        base.DisableModule();
        
        _inputReader.OnMovementEvent -= SetInputDir;
        _inputReader.OnJumpEvent -= OnJump;
    }

    public void SetEnableMove(bool value)
    {
        _canMove = value;
    }

    private void CalcMovement()
    {
        switch (Controller.Converter.AxisType)
        {
            case EAxisType.NONE:
                _moveVelocity = (_inputDir.x * MainCam.RightView() + _inputDir.z * MainCam.ForwardView()) * Controller.DataSO.walkSpeed;
                break;
            case EAxisType.Y:
                _moveVelocity = (_inputDir.x * MainCam.RightView() + _inputDir.z * MainCam.UpView()) * Controller.DataSO.walkSpeed;
                break;
            case EAxisType.X:
                _moveVelocity = new Vector3(0, 0, _inputDir.x) * Controller.DataSO.walkSpeed;
                break;
            case EAxisType.Z:
                _moveVelocity = new Vector3(_inputDir.x, 0, 0) * Controller.DataSO.walkSpeed;
                break;
        }
        
        _moveVelocity += _verticalVelocity;

        if (IsGround && _verticalVelocity.y < 0f)
        {
            _verticalVelocity.y = -1f;
        }
        else
        {
            _verticalVelocity.y += Controller.DataSO.gravity * Time.deltaTime;
        }
    }

    private bool CheckGround()
    {
        var size = _collider.size;
        size.y = 0.1f;
        var trm = transform;
        return Physics.BoxCast(
            trm.position,
            size,
            -trm.up,
            trm.rotation,
            _maxGroundCheckDistance,
            _groundMask
        );
    }

    public void StopImmediately()
    {
        _verticalVelocity.y = -1f;
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
            _verticalVelocity.y = Controller.DataSO.jumpPower;
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (_collider == null)
        {
            return;
        }

        var size = _collider.size;
        size.y = 0.1f;
        var trm = transform;
        Gizmos.DrawCube(trm.position - trm.up * _maxGroundCheckDistance, size);
    }
#endif
}
