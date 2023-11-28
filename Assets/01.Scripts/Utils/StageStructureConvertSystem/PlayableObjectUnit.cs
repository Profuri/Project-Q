using UnityEngine;

namespace StageStructureConvertSystem
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayableObjectUnit : StructureObjectUnitBase
    {
        [SerializeField] private LayerMask _standableObjectMask;
        [SerializeField] private float _rayDistance;

        private PlayerController _playerController;
        private CharacterController _characterController;

        public override void Init(StructureConverter converter)
        {
            base.Init(converter);
            _playerController = GetComponent<PlayerController>();
            _characterController = GetComponent<CharacterController>();
        }

        public override void TransformSynchronization(EAxisType axisType)
        {
            base.TransformSynchronization(axisType);
            _playerController.GetModule<PlayerMovementModule>().CanJump = axisType != EAxisType.Y;

            switch (axisType)
            {
                case EAxisType.NONE:
                    CheckObject(_prevObjectInfo.axis);
                    _characterController.center = Vector3.zero;
                    break;
                case EAxisType.X:
                    _objectInfo.position.x = 1;
                    _characterController.center = new Vector3(-1, 0, 0);
                    break;
                case EAxisType.Y:
                    _objectInfo.position.y = 1;
                    break;
                case EAxisType.Z:
                    _objectInfo.position.z = -1;
                    _characterController.center = new Vector3(0, 0, 1);
                    break;
            }
        }

        public override void ObjectSetting()
        {
            _characterController.enabled = false;
            base.ObjectSetting();
            _characterController.enabled = true;
        }

        private void CheckObject(EAxisType axisType)
        {
            var origin = _prevObjectInfo.position + _characterController.center;
            var dir = Vector3.down;

            RaycastHit hit;
            var isHit = Physics.Raycast(origin, dir, out hit, _rayDistance, _standableObjectMask);

            if (!isHit)
            {
                Debug.Log(1);
                return;
            }

            if (!hit.collider.TryGetComponent<PlatformObjectUnit>(out var unit))
            {
                Debug.Log(2);
                return;
            }

            Debug.Log(hit.collider.name);

            switch (axisType)
            {
                case EAxisType.X:
                    if (_objectInfo.position.x >= unit.ObjectInfo.position.x - unit.ObjectInfo.scale.x / 2f  &&
                        _objectInfo.position.x <= unit.ObjectInfo.position.x + unit.ObjectInfo.scale.x / 2f)
                    {
                        return;
                    }
                    _objectInfo.position.x = unit.ObjectInfo.position.x;
                    break;
                case EAxisType.Y:
                    if (_objectInfo.position.y >= unit.ObjectInfo.position.y + unit.ObjectInfo.scale.y / 2f + _objectInfo.scale.y / 2f)
                    {
                        return;
                    }
                    _objectInfo.position.y = unit.ObjectInfo.position.y + unit.ObjectInfo.scale.y / 2f + _objectInfo.scale.y / 2f;
                    break;
                case EAxisType.Z:
                    if (_objectInfo.position.z >= unit.ObjectInfo.position.z - unit.ObjectInfo.scale.z / 2f  &&
                        _objectInfo.position.z <= unit.ObjectInfo.position.z + unit.ObjectInfo.scale.z / 2f)
                    {
                        return;
                    }
                    _objectInfo.position.z = unit.ObjectInfo.position.z;
                    break;
            }
        }

        public override void ReloadObject()
        {
            _playerController.GetModule<PlayerMovementModule>().StopImmediately();
            base.ReloadObject();
        }
    }
}