using AxisConvertSystem;
using InputControl;
using InteractableSystem;
using UnityEngine;

public class PlayerUnit : ObjectUnit
{
    [SerializeField] private InputReader _inputReader;
    public InputReader InputReader => _inputReader;
    
    [SerializeField] private PlayerData _data;
    public PlayerData Data => _data;

    public Transform ModelTrm { get; private set; }
    public Animator Animator { get; private set; }
    private StateController _stateController;
    private PlayerUIController _playerUiController;

    public bool OnGround => CheckGround();

    private InteractableObject _selectedInteractableObject;

    public override void Awake()
    {
        base.Awake();
        Converter = GetComponent<AxisConverter>();
        ModelTrm = transform.Find("Model");
        Animator = ModelTrm.GetComponent<Animator>();
        _playerUiController = transform.Find("PlayerCanvas").GetComponent<PlayerUIController>();
        
        _stateController = new StateController(this);
        _stateController.RegisterState(new PlayerIdleState(_stateController, true, "Idle"));
        _stateController.RegisterState(new PlayerMovementState(_stateController, true, "Movement"));
        _stateController.RegisterState(new PlayerJumpState(_stateController, true, "Jump"));
        _stateController.RegisterState(new PlayerFallState(_stateController, true, "Fall"));
        _stateController.RegisterState(new PlayerAxisControlState(_stateController));
        _stateController.ChangeState(typeof(PlayerIdleState));
    }

    private void Update()
    {
        _stateController.UpdateState();

        _selectedInteractableObject = FindInteractable();
        _playerUiController.SetKeyGuide(_selectedInteractableObject is not null);
    }

    public override void OnPop()
    {
        _inputReader.OnInteractionEvent += OnInteraction;
    }

    public override void OnPush()
    {
        _inputReader.OnInteractionEvent -= OnInteraction;
    }

    public void SetVelocity(Vector3 velocity, bool useGravity = true)
    {
        if (useGravity)
        {
            velocity.y = Rigidbody.velocity.y;
        }
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
    
    private InteractableObject FindInteractable()
    {
        var cols = new Collider[_data.maxInteractableCnt];
        var size = Physics.OverlapSphereNonAlloc(Collider.bounds.center, _data.interactableRadius, cols, _data.interactableMask);

        for(var i = 0; i < size; ++i)
        {
            var interactable = cols[i].GetComponent<InteractableObject>();
            
            if (interactable is null)
            {
                continue;
            }
                
            if(interactable.InteractType == EInteractType.INPUT_RECEIVE)
            {
                return interactable;
            }
        }

        return null;
    }

    public void SetSection(Section section)
    {
        transform.SetParent(section.transform);
        Converter.Init(section);
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

    private void OnInteraction()
    {
        _selectedInteractableObject.OnInteraction(this, true);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        Gizmos.DrawWireSphere(Collider.bounds.center, _data.interactableRadius);
        
        if (_selectedInteractableObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Collider.bounds.center, _selectedInteractableObject.transform.position);
        }
    }
#endif
}