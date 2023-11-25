using UnityEngine;

namespace StageStructureConvertSystem
{
    public class GlassObjectUnit : PlatformObjectUnit
    {
        [SerializeField] private EAxisType _activeAxisType;

        public override void ObjectSetting()
        {
            _collider.enabled = _objectInfo.axis == _activeAxisType | _objectInfo.axis == EAxisType.NONE;
            base.ObjectSetting();
        }
    }
}