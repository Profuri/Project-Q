using InputControl;
using ModuleSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class AxisControlModule : BaseModule<PlayerController>
{
    [SerializeField] private InputReader _inputReader;

    private StructureConverter _converter;

    private bool _isControllingAxis;

    public override void Init(Transform root)
    {
        base.Init(root);
        _converter = Controller.Converter;
        _inputReader.OnAxisControlEvent += AxisControlHandle;
        _isControllingAxis = false;
    }

    public override void UpdateModule()
    {
        
    }

    public override void FixedUpdateModule()
    {
        
    }

    public override void DisableModule()
    {
        base.DisableModule();
        _inputReader.OnAxisControlEvent += AxisControlHandle;
    }

    private void AxisControlHandle()
    {
        _isControllingAxis = !_isControllingAxis;
        
        if (_isControllingAxis)
        {
            ChangeAxisControlState();
        }
        else
        {
            ChangeNormalState();
        }
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