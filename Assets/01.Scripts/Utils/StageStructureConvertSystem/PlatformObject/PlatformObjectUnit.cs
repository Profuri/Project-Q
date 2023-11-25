using UnityEngine;

namespace StageStructureConvertSystem
{
    public class PlatformObjectUnit : StructureObjectUnitBase
    {
        [SerializeField] private bool _isPlane = false;

        public override void ObjectSetting()
        {
            switch (_objectInfo.axis)
            {
                case EAxisType.X:
                    _objectInfo.material.renderQueue = Mathf.CeilToInt(_prevObjectInfo.position.x + 5f);
                    break;
                case EAxisType.Y:
                    _objectInfo.material.renderQueue = Mathf.CeilToInt(_prevObjectInfo.position.y);
                    break;
                case EAxisType.Z:
                    _objectInfo.material.renderQueue = Mathf.CeilToInt(Mathf.Abs(_prevObjectInfo.position.z - 5f));
                    break;
            }
            base.ObjectSetting();
        }

        public override void TransformSynchronization(EAxisType axisType)
        {
            base.TransformSynchronization(axisType);
            if (_isPlane)
            {
                _objectInfo.scale = new Vector3(10, 1, 10);
            }
        }
    }
}