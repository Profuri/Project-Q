using UnityEngine;

public class ObjectHoldingHandler : MonoBehaviour
{
    private PlayerUnit _player;
    
    private Transform _holdingPoint;
    
    private HoldableObject _heldObject;

    private readonly int _animationHoldHash = Animator.StringToHash("IsHold");

    private void Awake()
    {
        _player = GetComponent<PlayerUnit>();
        _holdingPoint = transform.Find("HoldingPoint");
    }

    private void Update()
    {
        HoldingPointMovement();
        if (_heldObject is not null)
        {
            _heldObject.SetPosition(_holdingPoint.position);
        }
    }

    public void Attach(HoldableObject obj)
    {
        _player.Animator.SetBool(_animationHoldHash, true);
        _heldObject = obj;
    }

    public void Detach()
    {
        _player.Animator.SetBool(_animationHoldHash, false);
        _heldObject = null;
    }

    private void HoldingPointMovement()
    {
        var holdPoint = _holdingPoint.position;
        var origin = _player.transform.position;
        origin.y = holdPoint.y;

        var originDir = (holdPoint - origin).normalized;  
        var destDir = _player.ModelTrm.forward;

        var lerpDir = Vector3.Lerp(originDir, destDir, _player.Data.holdingPointMoveSpeed);

        var destPos = origin + lerpDir * _player.Data.holdingRadius;

        _holdingPoint.position = destPos;
    }
}