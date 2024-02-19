using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using AxisConvertSystem;
using System.Linq;
public class GlassObjectUnit : ObjectUnit,IProvidableFieldInfo
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

    public List<FieldInfo> GetFieldInfos()
    {
        Type type = this.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        return fields.ToList();
    }

    public void SetFieldInfos(List<FieldInfo> infos)
    {
        
    }
}