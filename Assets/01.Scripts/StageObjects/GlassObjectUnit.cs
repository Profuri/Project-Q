using System;
using UnityEngine;
using StageStructureConvertSystem;

public class GlassObjectUnit : StructureObjectUnitBase
{
    [SerializeField] private EAxisType _activeAxisType;

    private Transform _visualTrm;

    public override void Init(StructureConverter converter)
    {
        _visualTrm = transform.Find("InnerObject");
        base.Init(converter);
    }

    public override void ObjectSetting()
    {
        gameObject.SetActive(_objectInfo.axis == _activeAxisType | _objectInfo.axis == EAxisType.NONE);

        if (_objectInfo.axis == _activeAxisType)
        {
            switch (_activeAxisType)
            {
                case EAxisType.X:
                    _visualTrm.localPosition = Vector3.right * 0.5f;
                    break;
                case EAxisType.Y:
                    _visualTrm.localPosition = Vector3.up * 0.5f;
                    break;
                case EAxisType.Z:
                    _visualTrm.localPosition = Vector3.back * 0.5f;
                    break;
            }
        }
        
        base.ObjectSetting();
    }
}