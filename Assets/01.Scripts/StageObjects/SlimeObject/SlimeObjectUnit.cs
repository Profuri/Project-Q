using System;
using System.Collections.Generic;
using AxisConvertSystem;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class SlimeObjectUnit : ObjectUnit
{
    [Header("SlimeSettings")]
    [SerializeField] private float _bouncePower = 5f;
    [SerializeField] private float _bounceTime = 0.5f;

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
                    Vector3 bounceDirection = Vector3.up;
                    unit.SetVelocity(bounceDirection * _bouncePower, false);
                }
            }
        }
        _canApply = false;
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
