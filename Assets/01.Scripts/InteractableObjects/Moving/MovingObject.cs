using System;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class MovingObject : InteractableObject
{
    [SerializeField] private Transform _movingTrackTrm;
    [SerializeField] private EMovingAxis _movingAxis;
    
    [SerializeField] private LayerMask _playerMask;

    [SerializeField] [Range(0.5f, 1f)] private float _checkDistance;

    private ObjectUnit _objectUnit;

    private float _minLimitPos;
    private float _maxLimitPos;
    
    private void OnEnable()
    {
        _objectUnit = GetComponent<ObjectUnit>();

        var scale = 0f;
        var trackScale = 0f;
        var origin = 0f;

        if (_movingAxis == EMovingAxis.X)
        {
            scale = transform.localScale.x;
            trackScale = _movingTrackTrm.localScale.x;
            origin = _movingTrackTrm.localPosition.x;
        }
        else
        {
            scale = transform.localScale.z;
            trackScale = _movingTrackTrm.localScale.z;
            origin = _movingTrackTrm.localPosition.z;
        }

        _minLimitPos = origin + -trackScale / 2f + scale / 2f;
        _maxLimitPos = origin + trackScale / 2f - scale / 2f;
    }

    private void Update()
    {
        // if (_objectUnit.UnitInfo.CompressType != AxisType.None)
        // {
            // return;
        // }
        
        // if (!CheckPlayer(out var cols))
        // {
            // return;
        // }
        
        // if (!cols[0].TryGetComponent<PlayableObjectUnit>(out var playerUnit))
        // {
            // return;
        // }   
            
        // OnInteraction(playerUnit, true);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        var player = ((PlayableObjectUnit)communicator).PlayerController;
        var playerMovementModule = player.GetModule<PlayerMovementModule>();

        if (!playerMovementModule.IsMovement)
        {
            return;
        }

        if (IsPulling(communicator.transform.position, playerMovementModule.MoveVelocity, out var dir))
        {
            var speed = player.DataSO.walkSpeed / 2f;
            transform.localPosition += dir * (speed * Time.deltaTime);
            ClampingPosition();
        }
    }

    private void ClampingPosition()
    {
        var pos = transform.localPosition;
        if (_movingAxis == EMovingAxis.X)
        {
            pos.x = Mathf.Clamp(pos.x, _minLimitPos, _maxLimitPos);
        }
        else
        {
            pos.z = Math.Clamp(pos.z, _minLimitPos, _maxLimitPos);
        }
        transform.localPosition = pos;
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
