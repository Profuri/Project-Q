using AxisConvertSystem;
using UnityEngine;

public class ObjectHoldingHandler : MonoBehaviour
{
    private PlayerUnit _player;
    private Transform _holdingPoint;
    private IHoldable _heldObject;

    public Vector3 HoldingPoint => _holdingPoint.position;

    private readonly int _animationHoldHash = Animator.StringToHash("IsHold");

    public bool IsHold => _heldObject != null;

    private void Awake()
    {
        _player = GetComponent<PlayerUnit>();
        _holdingPoint = transform.Find("HoldingPoint");
    }

    public void UpdateHandler()
    {
        HoldingPointMovement();
    }

    public void Attach(IHoldable obj)
    {
        if (IsHold) return;
       
        _player.Animator.SetBool(_animationHoldHash, true);
        _heldObject = obj;
        _heldObject.Attach(this);
    }

    public void Detach()
    {
        if (!IsHold) return;

        _player.Animator.SetBool(_animationHoldHash, false);
        _heldObject.Detach();
        _heldObject = null;
    }

    private void HoldingPointMovement()
    {
        var holdPoint = _holdingPoint.position;
        var origin = _player.transform.position;
        origin.y = holdPoint.y;

        var originDir = (holdPoint - origin).normalized;  
        var destDir = _player.ModelTrm.forward;

        var finalDir = _player.Converter.AxisType is AxisType.None or AxisType.Y ?
            Vector3.Lerp(originDir, destDir, _player.Data.holdingPointMoveSpeed) :
            destDir;

        var destPos = origin + finalDir * _player.Data.holdingRadius;

        _holdingPoint.position = destPos;
    }
}