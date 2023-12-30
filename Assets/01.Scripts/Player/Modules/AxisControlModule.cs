using InputControl;
using ModuleSystem;
using UnityEngine;

public class AxisControlModule : BaseModule<PlayerController>
{
    [SerializeField] private InputReader _inputReader;

    private bool _isControllingAxis;

    public override void Init(Transform root)
    {
        base.Init(root);
        _inputReader.OnAxisControlToggleEvent += AxisControlToggleHandle;
        _inputReader.OnClickEvent += SelectAxisHandle;
        _isControllingAxis = false;
    }

    public override void UpdateModule()
    {
        GetCurrentControlAxis();
    }

    public override void FixedUpdateModule()
    {
    }
    
    public override void DisableModule()
    {
        base.DisableModule();
        _inputReader.OnAxisControlToggleEvent += AxisControlToggleHandle;
        _inputReader.OnClickEvent -= SelectAxisHandle;
    }
    
    private EAxisType GetCurrentControlAxis()
    {
        var vCam = CameraManager.Instance.ActiveVCam;
        var camYValue = vCam.GetYValue();

        if (camYValue >= 0.5f)
        {
            return EAxisType.Y;
        }

        var lookDir = -CameraManager.Instance.MainCam.transform.forward;
        lookDir.y = 0;
        var angle = Mathf.Atan2(-lookDir.z, -lookDir.x) * Mathf.Rad2Deg;

        return GetLookAxis(angle);
    }

    private EAxisType GetLookAxis(float angle)
    {
        var isLookXAxis = angle is >= -180f and < -135f or > 135f and <= 180f;
        var isLookReverseXAxis = angle is > -45f and <= 0f or >= 0f and < 45f;
        var isLookZAxis = angle is >= 45f and <= 135f;
        var isLookReverseZAxis = angle is >= -135f and <= -45f;

        Debug.Log(isLookXAxis);
        Debug.Log(isLookZAxis);

        if (isLookXAxis)
            return EAxisType.X;
        if (isLookReverseXAxis)
            return EAxisType.X;
        if (isLookZAxis)
            return EAxisType.Z;
        if (isLookReverseZAxis)
            return EAxisType.Z;

        return EAxisType.NONE;
    }


    private void AxisControlToggleHandle(bool enter)
    {
        if (Controller.Converter.AxisType == EAxisType.NONE)
        {
            if (_isControllingAxis == enter)
            {
                return;
            }
            
            _isControllingAxis = enter;
            
            if (_isControllingAxis)
            {
                ChangeAxisControlState();
            }
            else
            {
                ChangeNormalState();
            }
        }
        else
        {
            if (!enter)
            {
                return;
            }
            
            Controller.ConvertDimension(EAxisType.NONE);
        }
    }

    private void SelectAxisHandle()
    {
        if (!_isControllingAxis)
        {
            return;
        }

        _isControllingAxis = false;
        var axis = GetCurrentControlAxis();
        ChangeNormalState();
        Controller.ConvertDimension(axis);
    }

    private void ChangeAxisControlState()
    {
        if (CameraManager.Instance.CurrentCamController is StageCamController)
        {
            ((StageCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(true);
        }
    }
    
    private void ChangeNormalState()
    {
        if (CameraManager.Instance.CurrentCamController is StageCamController)
        {
            ((StageCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(false);
        }
    }
}