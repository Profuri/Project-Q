using System;
using AxisConvertSystem;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayableObjectUnit : ObjectUnit
{
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

    public void ReloadObject()
    {
        // _playerController.GetModule<PlayerMovementModule>().StopImmediately();
        // _playerController.PlayerAnimatorController.UnActive();
        // _playerController.ConvertDimension(AxisType.None);
        // base.ReloadObject();
    }
}