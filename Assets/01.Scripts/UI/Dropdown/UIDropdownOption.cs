using System;
using TMPro;
using UnityEngine;

public class UIDropdownOption : UIComponent
{
    private string _optionName;
    private Action _callback;

    private TextMeshProUGUI _optionText;

    protected override void Awake()
    {
        base.Awake();
        _optionText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    public void Setting(string optionName, Action callback)
    {
        _optionName = optionName;
        _callback = callback;
        _optionText.text = _optionName;
    }

    public void Invoke()
    {
        _callback?.Invoke();
    }
}