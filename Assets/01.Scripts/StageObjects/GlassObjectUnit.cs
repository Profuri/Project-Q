using System;
using UnityEngine;
using AxisConvertSystem;

public class GlassObjectUnit : StructureObjectUnitBase
{
    [SerializeField] private AxisType _activeAxisType;

    private Transform _visualTrm;

    public override void Init(AxisConverter converter)
    {
        _visualTrm = transform.Find("InnerObject");
        base.Init(converter);
    }

    public override void ObjectSetting()
    {
        gameObject.SetActive(_objectInfo.axis == _activeAxisType | _objectInfo.axis == AxisType.None);

        if (_objectInfo.axis == _activeAxisType)
        {
            switch (_activeAxisType)
            {
                case AxisType.X:
                    _visualTrm.localPosition = Vector3.right * 0.5f;
                    break;
                case AxisType.Y:
                    _visualTrm.localPosition = Vector3.up * 0.5f;
                    break;
                case AxisType.Z:
                    _visualTrm.localPosition = Vector3.back * 0.5f;
                    break;
            }
        }
        
        base.ObjectSetting();
    }
}