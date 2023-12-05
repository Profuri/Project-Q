using System;
using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class MovingObject : InteractableObject
{
    [SerializeField] private Transform _movingTrackTrm;
    [SerializeField] private EMovingAxis _movingAxis;
    
    [SerializeField] private LayerMask _playerMask;

    [SerializeField] [Range(0.5f, 1f)] private float _checkDistance;

    private StructureObjectUnitBase _objectUnit;

    private float _minLimitPos;
    private float _maxLimitPos;
    
    private void Awake()
    {
        _objectUnit = GetComponent<StructureObjectUnitBase>();
        
        var scale = _movingAxis == EMovingAxis.X ? transform.localScale.x : transform.localScale.z;
        var trackScale = _movingAxis == EMovingAxis.X ? _movingTrackTrm.localScale.x : _movingTrackTrm.localScale.z;
        _minLimitPos = -trackScale / 2f + scale / 2f;
        _maxLimitPos = trackScale / 2f - scale / 2f;
    }

    private void Update()
    {
        if (_objectUnit.ObjectInfo.axis != EAxisType.NONE)
        {
            return;
        }
        
        if (!CheckPlayer(out var cols))
        {
            return;
        }
        
        if (!cols[0].TryGetComponent<PlayerController>(out var playerController))
        {
            return;
        }   
            
        OnInteraction(playerController, true);
    }

    public override void OnInteraction(PlayerController player, bool interactValue)
    {
        var playerMovementModule = player.GetModule<PlayerMovementModule>();

        if (!playerMovementModule.IsMovement)
        {
            return;
        }

        if (IsPulling(player.transform.position, playerMovementModule.MoveVelocity, out var dir))
        {
            var speed = player.DataSO.walkSpeed / 2f;
            transform.position += dir * (speed * Time.deltaTime);
            ClampingPosition();
        }
    }

    private void ClampingPosition()
    {
        var localPos = transform.localPosition;
        if (_movingAxis == EMovingAxis.X)
        {
            localPos.x = Mathf.Clamp(localPos.x, _minLimitPos, _maxLimitPos);
        }
        else
        {
            localPos.z = Math.Clamp(localPos.z, _minLimitPos, _maxLimitPos);
        }
        transform.localPosition = localPos;
    }

    private bool IsPulling(Vector3 playerOrigin, Vector3 movementVelocity, out Vector3 dir)
    {
        dir = movementVelocity;
        if (_movingAxis == EMovingAxis.X)
        {
            dir.z = 0;
        }
        else
        {
            dir.x = 0;
        }
        dir.y = 0;
        dir.Normalize();
        
        if (dir.sqrMagnitude <= 0f)
        {
            return false;
        }

        var position = transform.position;
        var distance = Vector3.Distance(position, playerOrigin);
        var playerDir = (position - playerOrigin).normalized;
        var playerPoint = position - (playerOrigin + dir * distance);
        var dotValue = Vector3.Dot(playerPoint, playerDir);

        return Mathf.Cos(dotValue) >= 0;
    }

    private bool CheckPlayer(out Collider[] cols)
    {
        var size = transform.localScale;
        if (_movingAxis == EMovingAxis.X)
        {
            size.x *= _checkDistance;
            size.z /= 2.25f;
        }
        else
        {
            size.z *= _checkDistance;
            size.x /= 2.25f;
        }
        size.y /= 2.25f;

        cols = new Collider[1];
        var colSize = Physics.OverlapBoxNonAlloc(transform.position, size, cols, transform.rotation, _playerMask);

        return colSize > 0;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var size = transform.localScale;
        if (_movingAxis == EMovingAxis.X)
        {
            size.x *= _checkDistance;
            size.z /= 2.25f;
        }
        else
        {
            size.z *= _checkDistance;
            size.x /= 2.25f;
        }
        size.y /= 2.25f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, size * 2f);
    }
#endif
}
