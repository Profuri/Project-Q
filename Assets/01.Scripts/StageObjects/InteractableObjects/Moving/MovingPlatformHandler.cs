using System;
using AxisConvertSystem;
using UnityEngine;

public class MovingPlatformHandler : MonoBehaviour
{
    private ObjectUnit _owner;
    private MovingTrack _track;
    
    public void Awake()
    {
        _owner = GetComponent<ObjectUnit>();
        _track = GetComponentInParent<MovingTrack>();
    }

    private void Start()
    {
        _owner.Rigidbody.mass = 5;
        _owner.Rigidbody.drag = 10;
        
        _owner.OnInitEvent += InitHandle;
        _owner.OnConvertEvent += ConvertHandle;
        _owner.OnApplyUnitInfoEvent += ApplyUnitInfoHandle;
    }

    private void OnDisable()
    {
        _owner.OnInitEvent -= InitHandle;
        _owner.OnConvertEvent -= ConvertHandle;
        _owner.OnApplyUnitInfoEvent -= ApplyUnitInfoHandle;
    }

    private void InitHandle(AxisConverter converter)
    {
        FreezeInvalidAxisMove();
    }

    private void ConvertHandle(AxisType axis)
    {
        if(axis == _track.MovingAxis)
        {
            _owner.ConvertedInfo.ColliderCenter.SetAxisElement(
                _track.MovingAxis,
                _owner.ConvertedInfo.ColliderCenter.GetAxisElement(_track.MovingAxis) - transform.localPosition.GetAxisElement(_track.MovingAxis)
            );
        }
        else if (axis == AxisType.Y)
        {
            _owner.ConvertedInfo.ColliderCenter.SetAxisElement(
                AxisType.Y,
                _owner.ConvertedInfo.ColliderCenter.GetAxisElement(AxisType.Y) - transform.localPosition.GetAxisElement(AxisType.Y)    
            );
        }
    }

    private void ApplyUnitInfoHandle(AxisType axis)
    {
        FreezeInvalidAxisMove();
    }

    private void FreezeInvalidAxisMove()
    {
        if (_track.MovingAxis == AxisType.X)
        {
            _owner.Rigidbody.FreezeAxisPosition(AxisType.Z);
        }
        else if(_track.MovingAxis == AxisType.Z)
        {
            _owner.Rigidbody.FreezeAxisPosition(AxisType.X);
        }
        _owner.Rigidbody.FreezeAxisPosition(AxisType.Y, false);
    }
    
    public void ClampingPosition(float halfDistance)
    {
        var pos = transform.localPosition;
        var diff = halfDistance - _owner.Collider.bounds.size.GetAxisElement(_track.MovingAxis) / 2f;
        pos.SetAxisElement(_track.MovingAxis, Mathf.Clamp(pos.GetAxisElement(_track.MovingAxis), -diff, diff));
        transform.localPosition = pos;
    }
}