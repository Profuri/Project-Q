using System;
using System.Collections.Generic;
using AxisConvertSystem;
using DG.Tweening;
using UnityEngine;

public class SlimeObjectUnit : ObjectUnit
{
    [Header("SlimeSettings")]
    [SerializeField] private float _bouncePower = 5f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private float _bounceTime = 0.5f;
    [SerializeField] private float _slimeMass = 0.1f;


    private AxisType _prevAxisType = AxisType.None;
    
    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);

        if (axis == AxisType.None && _prevAxisType == AxisType.Y)
        {
            var unitList = GetMovementUnit();

            ShowBounceEffect(axis, () =>
            {
                foreach (var unit in unitList)
                {
                    Debug.Log($"Unit: {unit}");
                    SlimeImpact(unit);
                }
            });
        }
        _prevAxisType = axis;
    }

    //이건 압축 해제되었을 때 팍 튕겨나가는거.
    private void SlimeImpact(ObjectUnit unit)
    {
        Vector3 bounceDirection = Vector3.up;
        unit.SetVelocity(bounceDirection * _bouncePower, false);
    }

    private void ShowBounceEffect(AxisType axisType, Action Callback)
    {
        Vector3 originScale = transform.localScale;
        Vector3 targetScale = transform.localScale;

        transform.localScale = targetScale;


        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originScale * 0.8f,_bounceTime * 0.5f)).SetEase(Ease.InBounce);
        seq.Append(transform.DOScale(originScale * 1.2f,_bounceTime * 0.5f)).SetEase(Ease.InBounce);
        seq.Append(transform.DOScale(originScale, _bounceTime * 0.5f)).SetEase(Ease.InBounce);
        seq.AppendCallback(() => Callback?.Invoke());
    }

    private List<ObjectUnit> GetMovementUnit()
    {
        List<ObjectUnit> unitList = new List<ObjectUnit>();
        //+ Vector3.up * Collider.bounds.size.y
        Vector3 checkCenterPos = Collider.bounds.center ;
        Vector3 halfExtents = Collider.bounds.extents * 0.5f + Vector3.up * 2f;
        Quaternion rotation = transform.rotation;

        Collider[] cols = Physics.OverlapBox(checkCenterPos, halfExtents, rotation);

        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                if (col.TryGetComponent(out ObjectUnit unit))
                {
                    if (!unit.staticUnit)
                    {
                        unitList.Add(unit);
                    }
                }
            }
        }
        return unitList;
    }



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        var col = GetComponent<Collider>();

        Vector3 checkCenterPos = transform.position + Vector3.up * Collider.bounds.size.y;
        Vector3 checkScale = Collider.bounds.size;

        Gizmos.DrawWireCube(checkCenterPos, checkScale);
    }
#endif
}
