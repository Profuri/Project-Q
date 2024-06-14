using System;
using System.Collections.Generic;
using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class PlayerUnit : ObjectUnit
{
    [SerializeField] private PlayerData _data;
    public PlayerData Data => _data;

    public Transform ModelTrm { get; private set; }
    public Animator Animator { get; private set; }
    public ObjectHoldingHandler HoldingHandler { get; private set; }
    public PlayerInteractHandler InteractHandler { get; private set; }

    private ObjectUnit _standingUnit;
    public ObjectUnit StandingUnit
    {
        get => _standingUnit;
        set
        {
            if (value is not null)
            {
                value.Collider.excludeLayers |= 1 << gameObject.layer;
            }
            _standingUnit = value;
        }
    }

    public SoundEffectPlayer SoundEffectPlayer { get; private set; }

    private StateController _stateController;

    private readonly int _activeHash = Animator.StringToHash("Active");

    private float _coyoteTime = 0f;

    private bool IsCoyote
    {
        get
        {
            bool isCoyote = Time.time - _coyoteTime < Data.coyoteTime;
            return isCoyote;
        }
    }

    public bool CanJump => OnGround || IsCoyote;
    public bool CanAxisControl { get; set; } = true;
    public bool IsControllingAxis => _stateController.CurrentState is PlayerAxisControlState;


    [SerializeField] private float _stepY = 0.05f;
    [SerializeField] private float _stepX = 0.03f;

    public void StartCoyoteTime()
    {
        _coyoteTime = Time.time;
    }   
    
    public void ResetCoyoteTime()
    {
        _coyoteTime = float.MinValue;
    }
    
    public override void Awake()
    {
        base.Awake();

        Converter = GetComponent<AxisConverter>();
        Converter.Player = this;
        ModelTrm = transform.Find("Model");
        Animator = ModelTrm.GetComponent<Animator>();
        HoldingHandler = GetComponent<ObjectHoldingHandler>();
        InteractHandler = GetComponent<PlayerInteractHandler>();

        _stateController = new StateController(this);
        _stateController.RegisterState(new PlayerIdleState(_stateController, true, "Idle"));
        _stateController.RegisterState(new PlayerMovementState(_stateController, true, "Movement"));
        _stateController.RegisterState(new PlayerJumpState(_stateController, true, "Jump"));
        _stateController.RegisterState(new PlayerFallState(_stateController, true, "Fall"));
        _stateController.RegisterState(new PlayerAxisControlState(_stateController));

        SoundEffectPlayer = new SoundEffectPlayer(this);
        CanAxisControl = true;
    }    
    public override void UpdateUnit()
    {
        base.UpdateUnit();
        if (StandingUnit)
        {
            StandingCheck();
        }

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
        {
            StageManager.Instance.StageClear(this);
        }
        
        _stateController.UpdateState();
        HoldingHandler.UpdateHandler();
        InteractHandler.UpdateHandler();
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        base.ApplyUnitInfo(axis);

        if (axis == AxisType.Z)
        {
            Rotate(Quaternion.LookRotation(Vector3.right), 1f);
        }
        else if (axis == AxisType.X)
        {
            Rotate(Quaternion.LookRotation(Vector3.forward), 1f);
        }
    }

    public override void ReloadUnit(bool useDissolve = false, float dissolveTime = 2f, Action callBack = null)
    {
        if (HoldingHandler.IsHold)
            HoldingHandler.Detach();
        
        Converter.ConvertDimension(AxisType.None);
        
        base.ReloadUnit(true, dissolveTime, () =>
        {
            callBack?.Invoke();
            if (!StoryManager.Instance.IsPlay)
            {
                InputManagerHelper.OnRevivePlayer();
            }
        });
        
        InputManagerHelper.OnDeadPlayer();
        SoundManager.Instance.PlaySFX("PlayerDead");
        PlaySpawnVFX();
        SetActiveAnimation(true);
        _stateController.ChangeState(typeof(PlayerIdleState));
    }

    public override void OnPop()
    {
        InputManager.Instance.PlayerInputReader.OnInteractionEvent += InteractHandler.OnInteraction;
        InputManager.Instance.PlayerInputReader.OnReloadClickEvent += RestartStage;
        _stateController.ChangeState(typeof(PlayerIdleState));
        SetActiveAnimation(true);
    }
    
    public override void OnPush()
    {
        InputManager.Instance.PlayerInputReader.ClearInputEvent();
        SetActiveAnimation(false);
    }

    private void RestartStage()
    {
        StageManager.Instance.RestartStage(this);
    }

    public void Rotate(Quaternion rot, float speed = -1)
    {
        ModelTrm.rotation = Quaternion.Lerp(ModelTrm.rotation, rot,
            speed < 0 ? _data.rotationSpeed : speed);
    }

    private void StandingCheck()
    {
        if (!StandingUnit.Collider.bounds.Contains(Collider.bounds.center))
        {
            StandingUnit.Collider.excludeLayers ^= 1 << gameObject.layer;
            StandingUnit = null;
        }
    }

    public void SetSection(Section section)
    {
        transform.SetParent(section.transform);
        Section = section;
        section.ReloadSectionUnits();
        Converter.Init(section);

        OriginUnitInfo.LocalPos = section.PlayerResetPoint;
    }

    public void AnimationTrigger(string key)
    {
        _stateController.CurrentState.AnimationTrigger(key);
    }
    
    private void PlaySpawnVFX()
    {
        var spawnVFX = PoolManager.Instance.Pop("SpawnVFX") as PoolableVFX;
        var bounds = Collider.bounds;
        var position = transform.position;
        position.y = bounds.min.y;

        spawnVFX.SetPositionAndRotation(position, Quaternion.identity);
        spawnVFX.SetScale(new Vector3(bounds.size.x, 1, bounds.size.z));
        spawnVFX.Play();
    }

    public void SetActiveAnimation(bool active)
    {
        Animator.SetBool(_activeHash, active);
    }

    //계속 실행되니까 OnGround가 바뀌었을 때는 체크 안함
    public override void SetGravity(bool useGravityParam)
    {
        if(OnGround)
        {
            useGravity = true;
            return;
        }        
        
        useGravity = useGravityParam;
    }

    public bool IsStairOnNextStep(out RaycastHit hit, float distance = 0.05f)
    {
        Vector3 dir = ModelTrm.forward;
        dir.y = 0f;

        Vector3 origin = Collider.bounds.center;
        origin.y = Collider.bounds.min.y;

        Vector3 bottomCenter = origin + Vector3.up * _stepY * 0.5f + dir * -_stepX;
        Vector3 halfExtents = Collider.bounds.extents;
        halfExtents.y = _stepY * 0.5f;

        int layer      = 1 << LayerMask.NameToLayer("Ground");

        bool bottomhit = Physics.BoxCast(bottomCenter, halfExtents, dir, out hit, Quaternion.identity, _stepX, layer);
        bool topDistance = hit.point.y < transform.position.y + _stepY;

        return bottomhit && topDistance;
    }

    private void OnDrawGizmos()
    {
        if (Collider == null) return;

        Vector3 dir = ModelTrm.forward;
        dir.y = 0f;

        Vector3 origin = Collider.bounds.center;
        origin.y = Collider.bounds.min.y;

        Vector3 topCenter = origin + Vector3.up * _stepY * 1.5f    + dir * -_stepX;
        Vector3 bottomCenter = origin + Vector3.up * _stepY * 0.5f + dir * -_stepX;
        Vector3 halfExtents = Collider.bounds.extents;
        halfExtents.y = _stepY * 0.5f;


        Gizmos.color = Color.red;
        Gizmos.DrawCube(topCenter,halfExtents);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(bottomCenter,halfExtents);
    }
}