using AxisConvertSystem;
using UnityEngine;

public class MovingPlatform : ObjectUnit
{
    private MovingTrack _track;
    
    public override void Awake()
    {
        base.Awake();
        _track = GetComponentInParent<MovingTrack>();
        Rigidbody.mass = 5;
        Rigidbody.drag = 10;
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        FreezeInvalidAxisMove();
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);
        FreezeInvalidAxisMove();
    }

    private void FreezeInvalidAxisMove()
    {
        if (_track.MovingAxis == AxisType.X)
        {
            Rigidbody.FreezeAxisPosition(AxisType.Z);
        }
        else if(_track.MovingAxis == AxisType.Z)
        {
            Rigidbody.FreezeAxisPosition(AxisType.X);
        }
        Rigidbody.FreezeAxisPosition(AxisType.Y, false);
    }
    
    public void ClampingPosition(float halfDistance)
    {
        var pos = transform.localPosition;
        var diff = halfDistance - Collider.bounds.size.GetAxisElement(_track.MovingAxis) / 2f;
        pos.SetAxisElement(_track.MovingAxis, Mathf.Clamp(pos.GetAxisElement(_track.MovingAxis), -diff, diff));
        transform.localPosition = pos;
    }
}