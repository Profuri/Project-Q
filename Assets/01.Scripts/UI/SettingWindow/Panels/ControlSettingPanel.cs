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
    [SerializeField] private UIButton3D _axisControlBtn;

    public void ChangeMoveFrontKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Movement, InputManager.OnlyAlphabet, 1);
    }
    
    public void ChangeMoveBackwardKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Movement, InputManager.OnlyAlphabet, 2);
    }
    
    public void ChangeMoveLeftKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Movement, InputManager.OnlyAlphabet, 3);
    }
    
    public void ChangeMoveRightKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Movement, InputManager.OnlyAlphabet, 4);   
    }
    
    public void ChangeJumpKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Jump, InputManager.AlphabetOrSpace);
    }
    
    public void ChangeInteractionKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.Interaction, InputManager.OnlyAlphabet);
    }
    
    public void ChangeAxisControlKeyBinding(UIButton3D caller)
    {
        ChangeKeyBinding(caller, EInputCategory.AxisControl, InputManager.OnlyAlphabet);
    }
    
    private void ChangeKeyBinding(UIButton3D caller, EInputCategory category, string bidingPattern, int bindingIndex = 0)
    {
        UIManager.Instance.Interact3DButton = false;
        
        var backgroundRenderer = caller.transform.Find("BackGroundPlane").GetComponent<Renderer>();
        backgroundRenderer.material = _controlButtonAccessMat;
        
        InputManager.Instance.ChangeKeyBinding(category, bidingPattern, bindingIndex, 
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
        string[] names = InputManager.Instance.GetBindingMovementName();

        for(int i = 0; i < 4; i++)
        {
            _movementBtnList[i].Text = names[i];
        } 

        _axisControlBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.AxisControl);
        _interactionBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.Interaction);
        _jumpBtn.Text = InputManager.Instance.GetBindingKeyName(EInputCategory.Jump);
    }


    public void SaveControls()
    {
        DataManager.Instance.SaveData(InputManager.Instance);
    }
}