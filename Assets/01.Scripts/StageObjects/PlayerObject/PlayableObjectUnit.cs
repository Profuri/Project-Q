using AxisConvertSystem;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayableObjectUnit : ObjectUnit
{
    [SerializeField] private LayerMask _standableObjectMask;
    [SerializeField] private float _rayDistance;

    private PlayerController _playerController;
    public PlayerController PlayerController => _playerController;
    
    private CharacterController _characterController;

    private Vector3 _colliderCenter; 

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _playerController = GetComponent<PlayerController>();
        _characterController = GetComponent<CharacterController>();
        _colliderCenter = _characterController.center;
        SetOriginPos();
    }

    private void SetOriginPos()
    {
        // _originPos = StageManager.Instance.CurrentStage.PlayerResetPoint;
    }

    public void TransformSynchronization(AxisType axisType)
    {
        // base.TransformSynchronization(axisType);
        // _playerController.GetModule<PlayerMovementModule>().CanJump = axisType != AxisType.Y;

        // switch (axisType)
        // {
            // case AxisType.None:
                // CheckObject(_prevObjectInfo.CompressType);
                // _characterController.center = _colliderCenter;
                // break;
            // case AxisType.X:
                // _objectInfo.LocalPos.x = 1f;
                // _characterController.center = _colliderCenter + new Vector3(-1, 0, 0);
                // break;
            // case AxisType.Y:
                // _objectInfo.LocalPos.y = 1f;
                // break;
            // case AxisType.Z:
                // _objectInfo.LocalPos.z = -1f;
                // _characterController.center = _colliderCenter + new Vector3(0, 0, 1);
                // break;
        // }
    }

    public void ObjectSetting()
    {
        _characterController.enabled = false;
        // base.ObjectSetting();
        _characterController.enabled = true;
    }

    private void CheckObject(AxisType axisType)
    {
        // Vector3 basePos = StageManager.Instance.CurrentStage.transform.position;
        // var origin = basePos + _prevObjectInfo.LocalPos + _characterController.center + Vector3.up * (_prevObjectInfo.LocalScale.y / 2f);
        // var dir = Vector3.down;

        // var isHit = Physics.Raycast(origin, dir, out var hit, _rayDistance, _standableObjectMask);

        // if (!isHit)
        // {
            // return;
        // }

        // if (!hit.collider.TryGetComponent<ObjectUnit>(out var unit))
        // {
            // return;
        // }

        // switch (axisType)
        // {
            // case AxisType.X:
                // if (_objectInfo.LocalPos.x >= unit.ObjectInfo.LocalPos.x - unit.ObjectInfo.LocalScale.x / 2f  &&
                    // _objectInfo.LocalPos.x <= unit.ObjectInfo.LocalPos.x + unit.ObjectInfo.LocalScale.x / 2f)
                // {
                    // return;
                // }
                // _objectInfo.LocalPos.x = unit.ObjectInfo.LocalPos.x;
                // break;
            // case AxisType.Y:
                // if (_objectInfo.LocalPos.y >= unit.ObjectInfo.LocalPos.y + unit.ObjectInfo.LocalScale.y / 2f + _objectInfo.LocalScale.y / 2f)
                // {
                    // return;
                // }
                // _objectInfo.LocalPos.y = unit.ObjectInfo.LocalPos.y + unit.ObjectInfo.LocalScale.y / 2f + _objectInfo.LocalScale.y / 2f;
                // break;
            // case AxisType.Z:
                // if (_objectInfo.LocalPos.z >= unit.ObjectInfo.LocalPos.z - unit.ObjectInfo.LocalScale.z / 2f  &&
                    // _objectInfo.LocalPos.z <= unit.ObjectInfo.LocalPos.z + unit.ObjectInfo.LocalScale.z / 2f)
                // {
                    // return;
                // }
                // _objectInfo.LocalPos.z = unit.ObjectInfo.LocalPos.z;
                // break;
        // }
    }

    public void ReloadObject()
    {
        // _playerController.GetModule<PlayerMovementModule>().StopImmediately();
        // _playerController.PlayerAnimatorController.UnActive();
        // _playerController.ConvertDimension(AxisType.None);
        // base.ReloadObject();
    }
}