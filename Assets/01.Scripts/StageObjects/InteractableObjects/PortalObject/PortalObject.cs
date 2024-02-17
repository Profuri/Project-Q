using System;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class PortalObject : InteractableObject
{
    [SerializeField] private PortalObject _linkedPortal;
    [SerializeField] private AxisType _portalAxis;

    [SerializeField] private float _portalOutDistance = 1f;
    
    private ObjectUnit _parentUnit;

    public override void Awake()
    {
        base.Awake();
        _parentUnit = transform.parent.GetComponent<ObjectUnit>();
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (communicator is PlayerUnit playerUnit)
        {
            var destPos = _linkedPortal._parentUnit.transform.localPosition;
            destPos.SetAxisElement(_linkedPortal._portalAxis, destPos.GetAxisElement(_linkedPortal._portalAxis) * _portalOutDistance);
            
            playerUnit.SetPosition(destPos);
        }
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        var scale = Vector3.one;
        scale.SetAxisElement(_portalAxis, 0);
        var pos = Vector3ExtensionMethod.GetAxisDir(_portalAxis);
        pos.SetAxisElement(_portalAxis, pos.GetAxisElement(_portalAxis) * 0.51f);
        
        transform.localScale = scale;
        transform.localPosition = pos;
    }

#endif
}
