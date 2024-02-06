using System;
using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class PortalObject : InteractableObject
{
    [SerializeField] private PortalObject _linkedPortal;
    [SerializeField] private EAxisType _portalAxis;

    [SerializeField] private float _portalOutDistance = 1f;
    
    private StructureObjectUnitBase _parentUnit;

    private void Awake()
    {
        _parentUnit = transform.parent.GetComponent<StructureObjectUnitBase>();
    }

    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        if (communicator is PlayableObjectUnit playerUnit)
        {
            var destPos = _linkedPortal._parentUnit.transform.localPosition;
            var dir = Vector3.zero;
            
            switch (_linkedPortal._portalAxis)
            {
                case EAxisType.X:
                    dir = Vector3.right * _portalOutDistance;
                    break;
                case EAxisType.Y:
                    dir = Vector3.up * _portalOutDistance;
                    break;
                case EAxisType.Z:
                    dir = Vector3.back * _portalOutDistance;
                    break;
            }

            playerUnit.SetObjectInfo(
                destPos + dir,
                playerUnit.transform.rotation,
                playerUnit.transform.localScale
            );
        }
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        switch (_portalAxis)
        {
            case EAxisType.X:
                transform.localScale = new Vector3(0, 1, 1);
                transform.localPosition = new Vector3(0.51f, 0f, 0f);
                break;
            case EAxisType.Y:
                transform.localScale = new Vector3(1, 0, 1);
                transform.localPosition = new Vector3(0f, 0.51f, 0f);
                break;
            case EAxisType.Z:
                transform.localScale = new Vector3(1, 1, 0);
                transform.localPosition = new Vector3(0f, 0f, -0.51f);
                break;
        }
    }

#endif
}
