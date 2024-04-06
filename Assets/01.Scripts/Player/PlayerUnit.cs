using System;
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
    public ObjectUnit StandingUnit { get; set; }
    private StateController _stateController;

    private InteractableObject _selectedInteractableObject;
    
    private readonly int _activeHash = Animator.StringToHash("Active");
    
    public override void Awake()
    {
        base.Awake();

        Converter = GetComponent<AxisConverter>();
        Converter.Player = this;
        ModelTrm = transform.Find("Model");
        Animator = ModelTrm.GetComponent<Animator>();
        HoldingHandler = GetComponent<ObjectHoldingHandler>();

        _stateController = new StateController(this);
        _stateController.RegisterState(new PlayerIdleState(_stateController, true, "Idle"));
        _stateController.RegisterState(new PlayerMovementState(_stateController, true, "Movement"));
        _stateController.RegisterState(new PlayerJumpState(_stateController, true, "Jump"));
        _stateController.RegisterState(new PlayerFallState(_stateController, true, "Fall"));
        _stateController.RegisterState(new PlayerAxisControlState(_stateController));
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();

        if (StandingUnit)
        {
            StandingCheck();
        }
        
        _stateController.UpdateState();

        _selectedInteractableObject = FindInteractable();

        //Test code
        if(Input.GetKeyDown(KeyCode.C))
        {
            StageManager.Instance.StageClear(this);
        }
    }

    public override void ReloadUnit(float dissolveTime = 2f, Action callBack = null)
    {
        Converter.ConvertDimension(AxisType.None);
        
        base.ReloadUnit(dissolveTime, () =>
        {
            callBack?.Invoke();
            InputManagerHelper.OnRevivePlayer();
        });
        
        InputManagerHelper.OnDeadPlayer();

        Converter.ConvertDimension(AxisType.None);
        Animator.SetBool(_activeHash, true);
        _stateController.ChangeState(typeof(PlayerIdleState));
    }

    public override void OnPop()
    {
        InputManager.Instance.PlayerInputReader.OnInteractionEvent += OnInteraction;
        InputManager.Instance.PlayerInputReader.OnReloadClickEvent += RestartStage;
        // 리펙토링 하자
        InputManager.Instance.CameraInputReader.OnChangeOffsetEvent += ChangeCameraOffset;

        _stateController.ChangeState(typeof(PlayerIdleState));
        Animator.SetBool(_activeHash, true);
    }
    
    public override void OnPush()
    {
        InputManager.Instance.PlayerInputReader.ClearInputEvent();
        // 리펙토링
        InputManager.Instance.CameraInputReader.ClearInputEvent();
        Animator.SetBool(_activeHash, false);
    }

    private void ChangeCameraOffset(Vector2 offset)
    {
        if (Mathf.Abs(offset.sqrMagnitude) <= 0)
        {
            CameraManager.Instance.ApplyInitOffset();    
        }
        else
        {
            CameraManager.Instance.ApplyCamOffset(offset);
        }
    }
    
    private void RestartStage()
    {
        if (Converter.AxisType != AxisType.None)
        {
            Converter.ConvertDimension(AxisType.None, () =>
            {
                StageManager.Instance.RestartStage(this);
            });
            return;
        }
        StageManager.Instance.RestartStage(this);
    }

    public void Rotate(Quaternion rot, float speed = -1)
    {
        ModelTrm.rotation = Quaternion.Lerp(ModelTrm.rotation, rot,
            speed < 0 ? _data.rotationSpeed : speed);
    }

    private void StandingCheck()
    {
        StandingUnit.Collider.excludeLayers |= 1 << gameObject.layer;
        
        if (!StandingUnit.Collider.bounds.Contains(Collider.bounds.center))
        {
            StandingUnit.Collider.excludeLayers ^= 1 << gameObject.layer;
            StandingUnit = null;
        }
    }

    private InteractableObject FindInteractable()
    {
        if (HoldingHandler.IsHold)
        {
            return null;
        }
        
        var cols = new Collider[_data.maxInteractableCnt];
        var size = Physics.OverlapSphereNonAlloc(Collider.bounds.center, _data.interactableRadius, cols, _data.interactableMask);

        for(var i = 0; i < size; ++i)
        {
            if (cols[i].TryGetComponent<InteractableObject>(out var interactable))
            {
                if(interactable.InteractType == EInteractType.INPUT_RECEIVE)
                {
                    var dir = (cols[i].bounds.center - Collider.bounds.center).normalized;
                    var isHit = Physics.Raycast(Collider.bounds.center - dir, dir, out var hit, Mathf.Infinity,
                        canStandMask);

                    if (isHit && cols[i] != hit.collider)
                    {
                        continue;
                    }

                    if (interactable != _selectedInteractableObject)
                    {
                        _selectedInteractableObject?.OnDetectedLeave();
                        interactable.OnDetectedEnter();
                    }
                    return interactable;
                }
            }
        }
        
        if (_selectedInteractableObject)
        {
            _selectedInteractableObject.OnDetectedLeave();
            _selectedInteractableObject = null;
        }

        return null;
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

    private void OnInteraction()
    {
        if (HoldingHandler.IsHold)
        {
            HoldingHandler.Detach();
            return;
        }
        
        if (_selectedInteractableObject is null)
        {
            return;
        }
        
        _selectedInteractableObject.OnInteraction(this, true);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        var col = GetComponent<Collider>();
        Gizmos.DrawWireSphere(col.bounds.center, _data.interactableRadius);
        
        if (_selectedInteractableObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(col.bounds.center, _selectedInteractableObject.transform.position);
        }
    }
#endif
}