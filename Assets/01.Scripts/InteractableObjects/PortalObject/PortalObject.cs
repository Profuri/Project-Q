using System;
using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class PortalObject : InteractableObject
{
    [SerializeField] private PortalObject _linkedPortal;
    [SerializeField] private EAxisType _portalAxis;

    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        if (communicator is PlayableObjectUnit playerUnit)
        {
            var destPos = _linkedPortal.transform.localPosition;

            switch (_linkedPortal._portalAxis)
            {
                case EAxisType.X:
                    destPos += Vector3.left;
                    break;
                case EAxisType.Y:
                    destPos += Vector3.up;
                    break;
                case EAxisType.Z:
                    destPos += Vector3.forward;
                    break;
            }
            
            playerUnit.SetObjectInfo(
                destPos,
                playerUnit.transform.rotation,
                playerUnit.transform.localScale
            );
        }
    }
}
