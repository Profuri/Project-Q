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

    private void Awake()
    {
        _parentUnit = transform.parent.GetComponent<ObjectUnit>();
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (communicator is PlayerUnit playerUnit)
        {
            var destPos = _linkedPortal._parentUnit.transform.localPosition;
            var dir = Vector3.zero;
            
            switch (_linkedPortal._portalAxis)
            {
                case AxisType.X:
                    dir = Vector3.right * _portalOutDistance;
                    break;
                case AxisType.Y:
                    dir = Vector3.up * _portalOutDistance;
                    break;
                case AxisType.Z:
                    dir = Vector3.back * _portalOutDistance;
                    break;
            }

            // playerUnit.SetObjectInfo(
                // destPos + dir,
                // playerUnit.transform.rotation,
                // playerUnit.transform.localScale
            // );
        }
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        switch (_portalAxis)
        {
            case AxisType.X:
                transform.localScale = new Vector3(0, 1, 1);
                transform.localPosition = new Vector3(0.51f, 0f, 0f);
                break;
            case AxisType.Y:
                transform.localScale = new Vector3(1, 0, 1);
                transform.localPosition = new Vector3(0f, 0.51f, 0f);
                break;
            case AxisType.Z:
                transform.localScale = new Vector3(1, 1, 0);
                transform.localPosition = new Vector3(0f, 0f, -0.51f);
                break;
        }
    }

#endif
}
