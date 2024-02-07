using System;
using AxisConvertSystem;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayableObjectUnit : ObjectUnit
{
    private PlayerController _playerController;
    public PlayerController PlayerController => _playerController;

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _playerController = GetComponent<PlayerController>();
        SetOriginPos();
    }

    protected override void UnitSetting(AxisType axis, UnitInfo unitInfo)
    {
        transform.localPosition = unitInfo.LocalPos + unitInfo.ColliderCenter;
        transform.localRotation = unitInfo.LocalRot;
        transform.localScale = unitInfo.LocalScale;

        var modelPos = Vector3.zero;
        modelPos.SetAxisElement(axis, unitInfo.LocalPos.GetAxisElement(axis));
        _playerController.ModelTrm.localPosition = modelPos;
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

    public void ReloadObject()
    {
        // _playerController.GetModule<PlayerMovementModule>().StopImmediately();
        // _playerController.PlayerAnimatorController.UnActive();
        // _playerController.ConvertDimension(AxisType.None);
        // base.ReloadObject();
    }
}