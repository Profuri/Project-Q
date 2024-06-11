using InputControl;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using ManagingSystem;

public enum EInputCategory
{
    Movement,
    Jump,
    Interaction,
    AxisControl,
    ZoomOut,
    Escape,
}

public class InputManager : BaseManager<InputManager>, IProvideSave, IProvideLoad
{
    [field:SerializeField] public PlayerInputReader PlayerInputReader { get; private set; }
    [field:SerializeField] public UIInputReader UIInputReader { get; private set; }
    [field:SerializeField] public CameraInputReader CameraInputReader { get; private set; }
    [field:SerializeField] public TimelineInputReader TimelineInputReader { get; private set; }

    private Dictionary<EInputCategory, InputAction> _inputDictionary;

    private const string InputBindableKeys = @"^(?:[a-zA-Z]|Space|Tap)$";

    public override void StartManager()
    {
        SettingDictionary();
        DataManager.Instance.SettingDataProvidable(this, this);
    }

    private void SettingDictionary()
    {
        _inputDictionary = new Dictionary<EInputCategory, InputAction>
        {
            { EInputCategory.Escape,            UIInputReader.Actions.Pause},
            { EInputCategory.Movement,          PlayerInputReader.Actions.Movement },
            { EInputCategory.Jump,              PlayerInputReader.Actions.Jump },
            { EInputCategory.Interaction,       PlayerInputReader.Actions.Interaction },
            { EInputCategory.AxisControl,       PlayerInputReader.Actions.AxisControl },
            { EInputCategory.ZoomOut,           CameraInputReader.Actions.ZoomControl },
        };
    }

    public void ChangeKeyBinding(
        EInputCategory category,
        int bindingIndex,
        Action<InputActionRebindingExtensions.RebindingOperation> onCancel,
        Action<InputActionRebindingExtensions.RebindingOperation> onComplete)
    {
        PlayerInputReader.Actions.Disable();
        _inputDictionary[category].PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<keyboard>/Backspace")
            .OnComplete(operation =>
            {
                var keyString = operation.action.bindings[bindingIndex].ToDisplayString();
                if (InvalidBindingKey(keyString, InputBindableKeys))
                {
                    operation.Cancel();
                    return;
                }
                
                onComplete?.Invoke(operation);
                operation.Dispose();
                PlayerInputReader.Actions.Enable();
            })
            .OnCancel(operation =>
            {
                onCancel?.Invoke(operation);
                operation.Dispose();
                PlayerInputReader.Actions.Enable();
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
            saveData.KeyBinding = PlayerInputReader.InputControls.SaveBindingOverridesAsJson();
        };
    }

    public Action<SaveData> GetLoadAction()
    {
        return (saveData) =>
        {
            if (string.IsNullOrEmpty(saveData.KeyBinding)) return;

            PlayerInputReader.InputControls.LoadBindingOverridesFromJson(saveData.KeyBinding);
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
        names[0] = PlayerInputReader.Actions.Movement.bindings[1].ToDisplayString();
        names[1] = PlayerInputReader.Actions.Movement.bindings[2].ToDisplayString();
        names[2] = PlayerInputReader.Actions.Movement.bindings[3].ToDisplayString();
        names[3] = PlayerInputReader.Actions.Movement.bindings[4].ToDisplayString();
        return names;
    }
}


public static class InputManagerHelper
{
    public static void OnControllingAxis()
    {
        EInputCategory[] inputs = { EInputCategory.AxisControl,  };
        InputManager.Instance.SetEnableInputWithout(inputs, true);
    }
    public static void OnCancelingAxis() => InputManager.Instance.SetEnableInputAll(true);
    public static void OnDeadPlayer() => InputManager.Instance.SetEnableInputAll(false);
    public static void OnRevivePlayer() => InputManager.Instance.SetEnableInputAll(true);
}
