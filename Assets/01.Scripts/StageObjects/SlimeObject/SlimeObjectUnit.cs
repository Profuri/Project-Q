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

    public override void OnCameraSetting(AxisType axis)
    {
        base.OnCameraSetting(axis);
        
        if(_affectedUnits.Count > 0)
        { 
            ApplyBounceEffect();
        }
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        base.ApplyUnitInfo(axis);
        
        _prevAxisType = axis;
    }

    private void ApplyBounceEffect()
    {
        foreach (var unit in _affectedUnits)
        {
            Vector3 bounceDirection = Vector3.up;
            unit.SetVelocity(bounceDirection * _bouncePower, false);
        }
        _affectedUnits.Clear();
    }
    
    private List<ObjectUnit> GetMovementUnit()
    {
        List<ObjectUnit> unitList = new List<ObjectUnit>();

        Vector3 checkCenterPos = Collider.bounds.center;
        Vector3 halfExtents = Collider.bounds.extents;
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
