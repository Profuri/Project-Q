using InputControl;
using Singleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using ManagingSystem;
using static InputControl.InputControls;

public enum EInputCategory
{
    Movement,
    Jump,
    Interaction,
    Click,
    AxisControl,
}

public class InputManager : BaseManager<InputManager>, IProvideSave, IProvideLoad
{
    [field:SerializeField] public InputReader InputReader { get; private set; }

    private Dictionary<EInputCategory, InputAction> _inputDictionary;

    public static readonly string AnyKey = @"^.*$";
    public static readonly string OnlyAlphabet = @"^[a-zA-Z]$";
    public static readonly string AlphabetOrSpace = @"^(?:[a-zA-Z]|Space)$";

    public override void StartManager()
    {
        SettingDictionary();
        DataManager.Instance.SettingDataProvidable(this, this);
    }

    private void SettingDictionary()
    {
        _inputDictionary = new Dictionary<EInputCategory, InputAction>
        {
            { EInputCategory.Movement,          InputReader.InputControls.Player.Movement },
            { EInputCategory.Jump,              InputReader.InputControls.Player.Jump },
            { EInputCategory.Interaction,       InputReader.InputControls.Player.Interaction },
            { EInputCategory.Click,             InputReader.InputControls.Player.Click },
            { EInputCategory.AxisControl,       InputReader.InputControls.Player.AxisControl },
        };
    }

    public void ChangeKeyBinding(
        EInputCategory category,
        string bidingPattern,
        int bindingIndex,
        Action<InputActionRebindingExtensions.RebindingOperation> onCancel,
        Action<InputActionRebindingExtensions.RebindingOperation> onComplete)
    {
        InputReader.InputControls.Player.Disable();
        _inputDictionary[category].PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<keyboard>/Backspace")
            .OnComplete(operation =>
            {
                var keyString = operation.action.bindings[bindingIndex].ToDisplayString();
                if (InvalidBindingKey(keyString, bidingPattern))
                {
                    operation.Cancel();
                    return;
                }
                
                onComplete?.Invoke(operation);
                operation.Dispose();
                InputReader.InputControls.Player.Enable();
            })
            .OnCancel(operation =>
            {
                onCancel?.Invoke(operation);
                operation.Dispose();
                InputReader.InputControls.Player.Enable();
            })
            .Start();
    }

    private bool InvalidBindingKey(string key, string pattern)
    {
        var regex = new Regex(pattern);
        return !regex.Match(key).Success;
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

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            saveData.KeyBinding = InputReader.InputControls.SaveBindingOverridesAsJson();
        };
    }

    public Action<SaveData> GetLoadAction()
    {
        return (saveData) =>
        {
            if (saveData.KeyBinding == null || saveData.KeyBinding == string.Empty) return;

            InputReader.InputControls.LoadBindingOverridesFromJson(saveData.KeyBinding);
        };
    }
    public string GetBindingKeyName(EInputCategory inputCategory)
    {
        int index = _inputDictionary[inputCategory].GetBindingIndex();
        return _inputDictionary[inputCategory].bindings[index].ToDisplayString();
    }
    public string[] GetBindingMovementName()
    {
        string[] names = new string[4];
        names[0] = InputReader.InputControls.Player.Movement.bindings[1].ToDisplayString();
        names[1] = InputReader.InputControls.Player.Movement.bindings[2].ToDisplayString();
        names[2] = InputReader.InputControls.Player.Movement.bindings[3].ToDisplayString();
        names[3] = InputReader.InputControls.Player.Movement.bindings[4].ToDisplayString();
        return names;
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
