using System;
using UnityEngine;
using AxisConvertSystem;

public class GlassObjectUnit : ObjectUnit
{
    [SerializeField] private AxisType _activeAxisType;

    private Transform _visualTrm;

    public override void Awake()
    {
        base.Awake();
        _visualTrm = transform.Find("InnerObject");
    }

    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);
        
        gameObject.SetActive(axis == _activeAxisType | axis == AxisType.None);

        if (axis == _activeAxisType)
        {
            _visualTrm.localPosition = Vector3ExtensionMethod.GetAxisDir(axis) * 0.5f;
        }
    }
}