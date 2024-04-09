using System;
using System.Collections.Generic;
using TinyGiantStudio.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSettingPanel : WindowPanel
{
    [SerializeField] private Material _controlButtonDefaultMat;
    [SerializeField] private Material _controlButtonAccessMat;

    [SerializeField] private List<UIButton3D> _movementBtnList;
    [SerializeField] private UIButton3D _jumpBtn;
    [SerializeField] private UIButton3D _interactionBtn;
    [SerializeField] private UIButton3D _zoomOutBtn;
    [SerializeField] private UIButton3D _stageResetBtn;

    public void ChangeMoveFrontKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Movement, 1);
    }
    
    public void ChangeMoveBackwardKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Movement, 2);
    }
    
    public void ChangeMoveLeftKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Movement, 3);
    }
    
    public void ChangeMoveRightKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Movement, 4);   
    }
    
    public void ChangeJumpKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Jump);
    }
    
    public void ChangeInteractionKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Interaction);
    }
    
    public void ChangeZoomOutKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.ZoomOut);
    }
    
    public void ChangeStageResetKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Reload);
    }
    
    private void ChangeKeyBinding(UIButton3D caller, EInputCategory category, int bindingIndex = 0)
    {
        UIManager.Instance.Interact3DButton = false;
        
        var backgroundRenderer = caller.transform.Find("BackGroundPlane").GetComponent<Renderer>();
        backgroundRenderer.material = _controlButtonAccessMat;
        
        InputManager.Instance.ChangeKeyBinding(category, bindingIndex, 
            // On Cancel
            operation =>
            {
                backgroundRenderer.material = _controlButtonDefaultMat;
                UIManager.Instance.Interact3DButton = true;
            },
            // On Complete
            operation =>
            {
                caller.Text = operation.action.bindings[bindingIndex].ToDisplayString();
                backgroundRenderer.material = _controlButtonDefaultMat;
                UIManager.Instance.Interact3DButton = true;
            }
        );
    }


    public override void LoadPanel()
    {
        base.LoadPanel();
        DataManager.Instance.LoadData(InputManager.Instance);
        SettingUI();
    }

    public override void ReleasePanel()
    {
        base.ReleasePanel();
        DataManager.Instance.LoadData(InputManager.Instance);
    }

    private void SettingUI()
    {
        var movementKeynames = InputManager.Instance.GetBindingMovementName();
        for(var i = 0; i < 4; i++)
        {
            _movementBtnList[i].Text = movementKeynames[i];
        } 

        _zoomOutBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.ZoomOut);
        _stageResetBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.Reload);
        _interactionBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.Interaction);
        _jumpBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.Jump);
    }


    public void SaveControls()
    {
        DataManager.Instance.SaveData(InputManager.Instance);
    }
}