using AxisConvertSystem;
using InputControl;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerUnit : ObjectUnit
{
    [SerializeField] private InputReader _inputReader;
    public InputReader InputReader => _inputReader;
    
    [SerializeField] private PlayerData _data;
    public PlayerData Data => _data;

    public Transform ModelTrm { get; private set; }
    public Animator Animator { get; private set; }

    public bool OnGround => CheckGround();

    private StateController _stateController;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        if (_stateController is null)
        {
            _stateController = new StateController(this);
            _stateController.RegisterState(new PlayerIdleState(_stateController, true, "Idle"));
            _stateController.RegisterState(new PlayerMovementState(_stateController, true, "Movement"));
            _stateController.RegisterState(new PlayerJumpState(_stateController, true, "Jump"));
            _stateController.RegisterState(new PlayerAxisControlState(_stateController));
        }
        
        ModelTrm = transform.Find("ModelTrm");
        Animator = ModelTrm.GetComponent<Animator>();
        
        _stateController.ChangeState(typeof(PlayerIdleState));
    }

    private void Update()
    {
        _stateController.UpdateState();
    }

    public void SetVelocity(Vector3 velocity)
    {
        Rigidbody.velocity = velocity;
    }

    public void StopImmediately(bool withYAxis)
    {
        Rigidbody.velocity = withYAxis ? Vector3.zero : new Vector3(0, Rigidbody.velocity.y, 0);
    }

    public void Rotate(Quaternion rot, float speed = -1)
    {
        ModelTrm.rotation = Quaternion.Lerp(ModelTrm.rotation, rot, speed < 0 ? _data.rotationSpeed : speed);
    }
    
    private bool CheckGround()
    {
        var size = Collider.bounds.size * 0.8f;
        var center = Collider.bounds.center;
        size.y = 0.1f;
        var isHit = Physics.BoxCast(
            center,
            size,
            -transform.up,
            out var hit,
            ModelTrm.rotation,
            _data.groundCheckDistance,
            _data.groundMask
        );
        
        return isHit && !hit.collider.isTrigger;
    }

    private void SetOriginPos()
    {
        // _originPos = StageManager.Instance.CurrentStage.PlayerResetPoint;
    }

    public void ReloadObject()
    {
        // _playerController.GetModule<PlayerMovementModule>().StopImmediately();
        // _playerController.PlayerAnimatorController.UnActive();
        // _playerController.ConvertDimension(AxisType.None);
        // base.ReloadObject();
    }
}