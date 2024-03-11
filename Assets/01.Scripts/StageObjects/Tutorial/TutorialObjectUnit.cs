using AxisConvertSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectUnit : ObjectUnit
{
    [Header("Tutorial System")]
    [SerializeField] private TutorialSO _tutorialSO;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _findDistance = 2f;
    [SerializeField] private Transform _markAppearTransform;

    private TutorialMark _rotateTarget;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        LoadTutorialMark();
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);
        if(axis != AxisType.None)
        {
            SceneControlManager.Instance.DeleteObject(_rotateTarget);
            _rotateTarget = null;
        }
        else
        {
            if(_rotateTarget == null)
            {
                LoadTutorialMark();
            }
        }
    }

    private void LoadTutorialMark()
    {
        _rotateTarget = SceneControlManager.Instance.AddObject("TutorialMark") as TutorialMark;

        if (_markAppearTransform != null)
        {
            _rotateTarget.transform.position = _markAppearTransform.position;
        }
        else
        {
            _rotateTarget.transform.position = Collider.bounds.center + Vector3.up * 2.0f;
        }
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
