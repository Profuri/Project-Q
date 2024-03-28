using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSettingPanel : WindowPanel
{
    [SerializeField] private Material _controlButtonDefaultMat;
    [SerializeField] private Material _controlButtonAccessMat;
    
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
    }

    public override void ReleasePanel()
    {
        base.ReleasePanel();
    }
}