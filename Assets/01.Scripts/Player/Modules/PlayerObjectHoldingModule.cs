using ModuleSystem;
using UnityEngine;

public class PlayerObjectHoldingModule : BaseModule<PlayerController>
{
    [SerializeField] private Transform _holdPoint;
    
    private HoldableObject _heldObject;
    public HoldableObject HeldObject => _heldObject;
    
    public override void Init(Transform root)
    {
        base.Init(root);
        _heldObject = null;
    }

    public override void UpdateModule()
    {
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
}