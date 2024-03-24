using InputControl;
using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public enum EInputCategory
{
    Movement,
    Jump,
    Interaction,
    Click,
    AxisControl,
    X_CameraSwitcher,
    Y_CameraSwitcher,
    Z_CameraSwitcher,
    Reset,
}



public class InputManager : MonoSingleton<InputManager>
{
    [field:SerializeField] public InputReader InputReader { get; private set; }

    private Dictionary<EInputCategory, InputAction> _inputDictionary;

    private void Awake()
    {
        SettingDictionary();
    }

    private void SettingDictionary()
    {
        _inputDictionary = new Dictionary<EInputCategory, InputAction>
        {
            { EInputCategory.Movement,          InputReader.InputControls.Player.Movement },
            { EInputCategory.Jump,              InputReader.InputControls.Player.Jump },
            { EInputCategory.Interaction,       InputReader.InputControls.Player.Interaction },
            { EInputCategory.Click,              InputReader.InputControls.Player.Click },
            { EInputCategory.AxisControl,              InputReader.InputControls.Player.AxisControl },
            { EInputCategory.X_CameraSwitcher,  InputReader.InputControls.Editor.XCameraSwitcher },
            { EInputCategory.Y_CameraSwitcher,  InputReader.InputControls.Editor.YCameraSwitcher },
            { EInputCategory.Z_CameraSwitcher,  InputReader.InputControls.Editor.ZCameraSwitcher },
            { EInputCategory.Reset,             InputReader.InputControls.Editor.Reset }
        };
    }

    public void SetEnableInput(EInputCategory[] categories,bool enable)
    {
        foreach(EInputCategory category in categories)
        {
            if (enable)
            {
                _inputDictionary[category].Enable();
            }
            else
            {
                _inputDictionary[category].Disable();
            }
        }
    }
    
    
    /// <summary>
    /// if in categories => enable not categories => !enable
    /// </summary>
    /// <param name="categories"></param>
    /// <param name="enable"></param>

    public void SetEnableInputWithout(EInputCategory[] categories, bool enable)
    {
        foreach (EInputCategory category in Enum.GetValues(typeof(EInputCategory)))
        {
            bool isInCategories = categories.Contains(category);
            bool isEnable = isInCategories ? enable : !enable; 
            
            
            if (isEnable)
                _inputDictionary[category].Enable();
            else
                _inputDictionary[category].Disable();
        }
    }



    public void SetEnableInputWithout(EInputCategory cg, bool enable)
    {
        foreach (EInputCategory category in Enum.GetValues(typeof(EInputCategory)))
        {
            bool isEnable = enable;
            isEnable = category == cg ? isEnable : !isEnable;

            if (!isEnable)
            {
                _inputDictionary[category].Enable();
            }
            else
            {
                _inputDictionary[category].Disable();
            }
        }
    }

    public void SetEnableInput(EInputCategory category,bool enable)
    {
        if(enable)
        {
            _inputDictionary[category].Enable();
        }
        else
        {
            _inputDictionary[category].Disable();
        }
    }

    public void SetEnableInputAll(bool enable)
    {
        foreach(EInputCategory category in Enum.GetValues(typeof(EInputCategory)))
        {
            if(enable)
            {
                _inputDictionary[category].Enable();
            }
            else
            {
                _inputDictionary[category].Disable();
            }
        }
    }
}


public static class InputManagerHelper
{
    public static void OnControllingAxis()
    {
        EInputCategory[] inputs = { EInputCategory.AxisControl, EInputCategory.Click };
        InputManager.Instance.SetEnableInputWithout(inputs, true);
    }
    public static void OnCancelingAxis() => InputManager.Instance.SetEnableInputAll(true);

    public static void OnDeadPlayer() => InputManager.Instance.SetEnableInputAll(false);
    public static void OnRevivePlayer() => InputManager.Instance.SetEnableInputAll(true);
}
