using AxisConvertSystem;
using UnityEngine;
using DG.Tweening;


public class SlimeObjectUnit : ObjectUnit
{
    [Header("SlimeSettings")]
    [SerializeField] private Transform _targetTrm;
    [SerializeField] private float _jumpPower = 4f;
    private float _bounceTime = 0.5f;

    private const int _MAX_COLLIDER_CNT = 8;
    private AxisType _prevAxisType = AxisType.None;
    private bool _canApply = false;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _canApply = false;
    }

    public override void Convert(AxisType axis)
    {
        base.Convert(axis);

        if (axis == AxisType.None && _prevAxisType == AxisType.Y)
        {
            _canApply = true;
        }
    }
    
    public override void OnCameraSetting(AxisType axis)
    {
        base.OnCameraSetting(axis);
        
        if (_canApply)
        {
            ApplyBounceEffect(FindColliders());
        }
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        base.ApplyUnitInfo(axis);
        
        _prevAxisType = axis;
    }

    private void ApplyBounceEffect(Collider[] cols)
    {
        if (cols == null || cols.Length < 1) return;
        
        foreach (var col in cols)
        {
            if (col != null)
            {
                if (col.TryGetComponent(out ObjectUnit unit))
                {
                    MoveToTargetPos(unit);
                }
            }
        }
        _canApply = false;
    }

    private void MoveToTargetPos(ObjectUnit unit)
    {
        if (_targetTrm == null || unit.transform.Equals(transform)) return;

        const float baseDistance = 6f;
        float ratio = Vector3.Distance(_targetTrm.position, transform.position) / baseDistance;
        Debug.Log($"ratio: {ratio}");
        float bounceTime = _bounceTime * ratio;

        unit.transform.DOJump(_targetTrm.position,_jumpPower,1,bounceTime).SetEase(Ease.OutSine);
    }

    private Collider[] FindColliders()
    {
        Vector3 center = Collider.bounds.center + new Vector3(0,Collider.bounds.size.y  * 0.5f,0);
        Vector3 halfExtents = Collider.bounds.extents * 0.5f;
        Collider[] results = new Collider[_MAX_COLLIDER_CNT];
        Quaternion orientation = transform.rotation;
        
       int colCount = Physics.OverlapBoxNonAlloc(center,halfExtents,results,orientation);
       
        return results;
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
