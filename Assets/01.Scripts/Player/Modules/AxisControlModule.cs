using InputControl;
using ModuleSystem;
using UnityEngine;

public class AxisControlModule : BaseModule<PlayerController>
{
    [SerializeField] private InputReader _inputReader;

    private bool _isControllingAxis;
    private EAxisType _currentControlAxis;

    public override void Init(Transform root)
    {
        base.Init(root);
        _inputReader.OnAxisControlToggleEvent += AxisControlToggleHandle;
        _inputReader.OnClickEvent += SelectAxisHandle;
        _isControllingAxis = false;
    }

    public override void UpdateModule()
    {
        if(!_isControllingAxis)
        {
            return;
        }
        
        _currentControlAxis = GetCurrentControlAxis();
        LightManager.Instance.SetAxisLight(_currentControlAxis);
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

        if (camYValue >= 45f)
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
        var isLookZAxis = angle is >= 45f and <= 135f;

        if (isLookXAxis)
            return EAxisType.X;
        if (isLookZAxis)
            return EAxisType.Z;

        return EAxisType.NONE;
    }


    private void AxisControlToggleHandle(bool enter)
    {
        if (!Controller.Converter.Convertable)
        {
            return;
        }
        
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
        ChangeNormalState();
        Controller.ConvertDimension(_currentControlAxis);
    }

    private void ChangeAxisControlState()
    {
        if (CameraManager.Instance.CurrentCamController is SectionCamController)
        {
            VolumeManager.Instance.SetAxisControlVolume(true, 0.7f);
            ((SectionCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(true);
        }
    }
    
    private void ChangeNormalState()
    {
        if (CameraManager.Instance.CurrentCamController is SectionCamController)
        {
            VolumeManager.Instance.SetAxisControlVolume(false, 0.7f);
            LightManager.Instance.SetAxisLight(EAxisType.NONE);
            ((SectionCamController)CameraManager.Instance.CurrentCamController).SetAxisControlCam(false);
        }
    }
}