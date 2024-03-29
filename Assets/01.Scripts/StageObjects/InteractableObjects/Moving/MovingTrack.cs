using System;
using AxisConvertSystem;
using UnityEngine;

public class MovingTrack : ObjectUnit
{
    [SerializeField] private AxisType _movingAxis;
    public AxisType MovingAxis => _movingAxis;
    [SerializeField] private float _movingDistance;
    
    private MovingPlatform _movingPlatform;

    public override void Awake()
    {
        base.Awake();
        _movingPlatform = GetComponentInChildren<MovingPlatform>();
    }

    private void LateUpdate()
    {
        _movingPlatform.ClampingPosition(_movingDistance / 2f);
    }

    public override void UnitSetting(AxisType axis)
    {
        DepthHandler.InitDepth();
        base.UnitSetting(axis);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        var lineTrm = transform.Find("TrackLine");
        lineTrm.localScale = new Vector3(_movingDistance, 0.01f, 1f);
        
        var movingPlatformTrm = transform.Find("MovingPlatform");
        
        if (_movingAxis == AxisType.X)
        {
            lineTrm.localRotation = Quaternion.Euler(0, 0, 0);
            movingPlatformTrm.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (_movingAxis == AxisType.Z)
        {
            lineTrm.localRotation = Quaternion.Euler(0, 90, 0);
            movingPlatformTrm.localRotation = Quaternion.Euler(0, 90, 0);
        }
    }
#endif
}
