using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDropdown : UIComponent
{
    private List<UIDropdownOption> _options;

    private Transform _itemParentTrm;

    private RectTransform _cursor;
    private int _cursorIndex;

    private GridLayoutGroup _gridLayout;

    protected override void Awake()
    {
        base.Awake();
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
        _cursor.anchoredPosition3D = new Vector3(0, -(_cursorIndex * 60 + _gridLayout.spacing.y * _cursorIndex), 0);
    }

}