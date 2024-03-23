using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDropdown : UIComponent
{
    private List<UIDropdownOption> _options;

    private TextMeshProUGUI _titleText;

    private Transform _itemParentTrm;

    private RectTransform _cursor;
    private int _cursorIndex;
    private int _optionOffset;

    private GridLayoutGroup _gridLayout;

    private const int MaxShowOptionCount = 8;

    public string Title
    {
        get => _titleText.text;
        set => _titleText.text = value;
    }

    protected override void Awake()
    {
        base.Awake();
        _titleText = transform.Find("Header/Text").GetComponent<TextMeshProUGUI>();
        _itemParentTrm = transform.Find("Body/MainPanel/Items");
        _gridLayout = _itemParentTrm.GetComponent<GridLayoutGroup>();
        _cursor = (RectTransform)transform.Find("Body/MainPanel/Cursor");
        _options = new List<UIDropdownOption>();
    }

    public override void Appear(Transform parentTrm)
    {
        base.Appear(parentTrm);
        _options.Clear();
        _cursorIndex = 0;
        UpdateCursorPos();
        InputManager.Instance.InputReader.OnUpArrowClickEvent += CursorUp;
        InputManager.Instance.InputReader.OnDownArrowClickEvent += CursorDown;
        InputManager.Instance.InputReader.OnEnterClickEvent += EnterOption;
    }

    public override void Disappear()
    {
        InputManager.Instance.InputReader.OnUpArrowClickEvent -= CursorUp;
        InputManager.Instance.InputReader.OnDownArrowClickEvent -= CursorDown;
        InputManager.Instance.InputReader.OnEnterClickEvent -= EnterOption;
        
        foreach (var option in _options)
        {
            option.Disappear();
        }

        base.Disappear();
    }

    public void AddOption(string optionName, Action callback)
    {
        var option = UIManager.Instance.GenerateUI("DropdownOption", _itemParentTrm) as UIDropdownOption;
        option.Setting(optionName, callback);
        _options.Add(option);
    }

    private void CursorUp()
    {
        if (_cursorIndex - 1 < 0)
        {
            return;
        }

        _cursorIndex--;
        UpdateCursorPos();
    }

    private void CursorDown()
    {
        if (_cursorIndex + 1 >= _options.Count)
        {
            return;
        }

        _cursorIndex++;
        UpdateCursorPos();
    }

    private void EnterOption()
    {
        _options[_cursorIndex].Invoke();
        UIManager.Instance.RemoveTopUI();
    }

    private void UpdateCursorPos()
    {
        var cursorPosY = -(_cursorIndex * 60 + _gridLayout.spacing.y * _cursorIndex);
        _cursor.anchoredPosition3D = new Vector3(0, cursorPosY, 0);
    }

    private void CalcOptionOffset()
    {
        bool offsetDownState = _cursorIndex - _optionOffset < 0; 

        if (_cursorIndex - _optionOffset < 0)
        {
            
        }   
        else if (_cursorIndex - _optionOffset >= MaxShowOptionCount && _cursorIndex + _optionOffset + 1)
        {
            
        }
    }

    private void UpdateShowOptions()
    {
        
    }
}