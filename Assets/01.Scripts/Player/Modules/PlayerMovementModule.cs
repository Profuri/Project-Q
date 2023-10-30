using InputControl;
using ModuleSystem;
using UnityEngine;

public class PlayerMovementModule : BaseModule<PlayerController>
{
    [SerializeField] private InputReader _inputReader;
    
    private CharacterController _characterController;

    private Vector3 _inputDir;
    public Vector3 InputDir => _inputDir;
    
    private Vector3 _moveVelocity;
    private Vector3 _verticalVelocity;

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
        _moveVelocity = _inputDir * Controller.DataSO.walkSpeed;
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
