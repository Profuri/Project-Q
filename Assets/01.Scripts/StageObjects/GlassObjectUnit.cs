using UnityEngine;
using StageStructureConvertSystem;

public class GlassObjectUnit : PlatformObjectUnit
{
    [SerializeField] private EAxisType _activeAxisType;

    public override void ObjectSetting()
    {
        gameObject.SetActive(_objectInfo.axis == _activeAxisType | _objectInfo.axis == EAxisType.NONE);
        base.ObjectSetting();
    }
}