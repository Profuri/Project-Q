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
        if (!_isControllingAxis)
        {
            return;
        }

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

        var lookDir = -vCam.transform.forward;
        lookDir.y = 0;
        var angle = Mathf.Atan2(-lookDir.z, -lookDir.x);
        
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
        ChangeNormalState();
        var axis = GetCurrentControlAxis();
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