using AxisConvertSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectUnit : ObjectUnit
{
    [SerializeField] private TutorialSO _tutorialSO;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _findDistance = 2f;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();

        PlayerUnit playerUnit = FindPlayerUnit();
        if(TutorialManager.Instance.IsTutorialViewing)
        {
            if (playerUnit == null)
            {
                TutorialManager.Instance.StopTutorial();
            }
        }
        else
        {
            if(playerUnit != null)
            {
                TutorialManager.Instance.StartTutorial(_tutorialSO);
            }
        }
    }

    private PlayerUnit FindPlayerUnit()
    {
        Vector3 center = transform.position;
        Vector3 halfExtents = Collider.bounds.extents * 0.5f;
        Vector3 direction = Vector3.up;
        Quaternion quaternion = transform.rotation;
        float maxDistance = _findDistance;

        bool isHit = Physics.BoxCast(center,halfExtents,direction, out RaycastHit hit, quaternion,maxDistance,_layerMask);
        if(isHit)
        {
            if(hit.collider.TryGetComponent(out PlayerUnit playerUnit))
            {
                return playerUnit;
            }
        }
        return null;
    }
}
