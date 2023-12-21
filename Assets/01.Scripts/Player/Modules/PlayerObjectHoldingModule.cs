using ModuleSystem;
using UnityEngine;

public class PlayerObjectHoldingModule : BaseModule<PlayerController>
{
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private float _holdRadius;
    
    private HoldableObject _heldObject;
    public HoldableObject HeldObject => _heldObject;
    
    public override void Init(Transform root)
    {
        base.Init(root);
        _heldObject = null;
    }

    public override void UpdateModule()
    {
        HoldingPointMovement();
        
        if (_heldObject is null)
        {
            return;
        }

        _heldObject.transform.position = _holdPoint.position;
    }

    public override void FixedUpdateModule()
    {
        // Do Nothing
    }

    public void AttachObject(HoldableObject obj)
    {
        if (_heldObject is not null)
        {
            return;
        }
        
        _heldObject = obj;
    }

    public void DetachObject()
    {
        _heldObject = null;
    }

    private void HoldingPointMovement()
    {
        var holdPoint = _holdPoint.position;
        var height = _holdPoint.localPosition.y;
        var origin = Controller.transform.position + Controller.CharController.center + Vector3.up * height;

        var originDir = (holdPoint - origin).normalized;  
        var destDir = Controller.ModelTrm.forward;

        var lerpDir = Vector3.Lerp(originDir, destDir, Controller.DataSO.rotationSpeed * Time.deltaTime);

        var destPos = origin + lerpDir * _holdRadius;

        _holdPoint.position = destPos;
    }
}