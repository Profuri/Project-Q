using System.Collections.Generic;
using UnityEngine;

public class ControlSettingPanel : WindowPanel
{
    [SerializeField] private Material _controlButtonDefaultMat;
    [SerializeField] private Material _controlButtonAccessMat;

    [SerializeField] private List<UIButton3D> _movementBtnList;
    [SerializeField] private UIButton3D _jumpBtn;
    [SerializeField] private UIButton3D _interactionBtn;
    [SerializeField] private UIButton3D _zoomOutBtn;
    [SerializeField] private UIButton3D _stageResetBtn;

    public void ChangeMovementKeyBinding(int index)
    {
        ChangeKeyBinding(_movementBtnList[index], EInputCategory.Movement, index + 1);
    }
    
    public void ChangeJumpKeyBinding()
    {
        ChangeKeyBinding(_jumpBtn, EInputCategory.Jump);
    }
    
    public void ChangeInteractionKeyBinding()
    {
        ChangeKeyBinding(_interactionBtn, EInputCategory.Interaction);
    }
    
    public void ChangeZoomOutKeyBinding()
    {
        ChangeKeyBinding(_zoomOutBtn, EInputCategory.ZoomOut);
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
        //_stageResetBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.Reload);
        _interactionBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.Interaction);
        _jumpBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.Jump);
    }


    public void SaveControls()
    {
        SoundManager.Instance.PlaySFX("UIApply",false);
        DataManager.Instance.SaveData(InputManager.Instance);
    }
}