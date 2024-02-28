using System;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class MovingObject : InteractableObject
{
    [SerializeField] private Transform _movingTrackTrm;
    [SerializeField] private AxisType _movingAxis;
    
    [SerializeField] private LayerMask _playerMask;

    [SerializeField] [Range(0.5f, 1f)] private float _checkDistance;

    private float _minLimitPos;
    private float _maxLimitPos;
    
    public override void Awake()
    {
        base.Awake();

        var scale = transform.localScale.GetAxisElement(_movingAxis);
        var trackScale = _movingTrackTrm.localScale.GetAxisElement(_movingAxis);
        var origin = _movingTrackTrm.localPosition.GetAxisElement(_movingAxis);

        _minLimitPos = origin + -trackScale / 2f + scale / 2f;
        _maxLimitPos = origin + trackScale / 2f - scale / 2f;
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();

        if (!CheckPlayer(out var cols))
        {
            return;
        }
        
        if (!cols[0].TryGetComponent<PlayerUnit>(out var playerUnit))
        {
            return;
        }   
            
        OnInteraction(playerUnit, true);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        var player = (PlayerUnit)communicator;

        if (IsPulling(player.transform.position, player.Rigidbody.velocity, out var dir))
        {
            var speed = player.Data.walkSpeed / 2f;
            SetPosition(transform.position + dir * (speed * Time.deltaTime));
            ClampingPosition();
        }
    }

    private void ClampingPosition()
    {
        var pos = transform.localPosition;
        pos.SetAxisElement(_movingAxis, Mathf.Clamp(pos.GetAxisElement(_movingAxis), _minLimitPos, _maxLimitPos));
        transform.localPosition = pos;
    }

    private bool IsPulling(Vector3 playerOrigin, Vector3 movementVelocity, out Vector3 dir)
    {
        dir = Vector3.zero;
        dir.SetAxisElement(_movingAxis, movementVelocity.GetAxisElement(_movingAxis));
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
        if (_movingAxis == AxisType.X)
        {
            size.x *= _checkDistance;
            size.z /= 2.25f;
        }
        else if(_movingAxis == AxisType.Z)
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
        if (_movingAxis == AxisType.X)
        {
            size.x *= _checkDistance;
            size.z /= 2.25f;
        }
        else if(_movingAxis == AxisType.Z)
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
