using System.Collections.Generic;
using AxisConvertSystem;
using DG.Tweening;
using UnityEngine;

public class SlimeObjectUnit : ObjectUnit
{
    [Header("SlimeSettings")]
    [SerializeField] private float _bouncePower = 5f;
    [SerializeField] private float _bounceTime = 0.5f;

    private List<ObjectUnit> _affectedUnits;
    private AxisType _prevAxisType = AxisType.None;

    public override void Awake()
    {
        base.Awake();
        _affectedUnits = new List<ObjectUnit>();
    }

    public override void Convert(AxisType axis)
    {
        base.Convert(axis);
        if (axis == AxisType.None && _prevAxisType == AxisType.Y)
        {
            _affectedUnits = GetMovementUnit();
        }
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);
        
        if(_affectedUnits.Count > 0)
        {
            ShowBounceEffect();
        }
            
        _prevAxisType = axis;
    }
    
    //이건 압축 해제되었을 때 팍 튕겨나가는거.
    private void SlimeImpact(ObjectUnit unit)
    {
        Vector3 bounceDirection = Vector3.up;
        unit.SetVelocity(bounceDirection * _bouncePower, false);
    }

    private void ShowBounceEffect()
    {
        Vector3 originScale = transform.localScale;
        Vector3 targetScale = transform.localScale;

        transform.localScale = targetScale;

        InputManager.Instance.SetEnableInputAll(false);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originScale * 0.8f,_bounceTime * 0.5f)).SetEase(Ease.InBounce);
        seq.Append(transform.DOScale(originScale * 1.2f,_bounceTime * 0.5f)).SetEase(Ease.InBounce);
        seq.Append(transform.DOScale(originScale, _bounceTime * 0.5f)).SetEase(Ease.InBounce);
        seq.AppendCallback(() =>
        {
            foreach (var unit in _affectedUnits)
            {
                SlimeImpact(unit);
            }
            _affectedUnits.Clear();
            InputManager.Instance.SetEnableInputAll(true);
        });
    }

    private List<ObjectUnit> GetMovementUnit()
    {
        List<ObjectUnit> unitList = new List<ObjectUnit>();

        Vector3 checkCenterPos = Collider.bounds.center;
        Vector3 halfExtents = Collider.bounds.size / 2f;
        Quaternion rotation = transform.rotation;

        Collider[] cols = Physics.OverlapBox(checkCenterPos, halfExtents, rotation);

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
        
        return unitList;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        var col = GetComponent<Collider>();

        if(col != null)
        {
            Vector3 checkCenterPos = col.bounds.center;
            Vector3 checkScale = col.bounds.size;

            Gizmos.DrawWireCube(checkCenterPos, checkScale);
        }
    }
#endif
}
