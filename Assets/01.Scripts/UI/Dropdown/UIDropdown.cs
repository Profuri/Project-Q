using System;
using System.Collections.Generic;
using InputControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDropdown : UIComponent
{
    private List<UIDropdownOption> _options;

    private TextMeshProUGUI _titleText;

    private RectTransform _bodyTrm;
    private Transform _itemParentTrm;

    private RectTransform _cursor;
    private int _cursorIndex;
    private int _optionOffset;

    private GridLayoutGroup _gridLayout;

    private int _maxShowOptionCount;

    private InputReader.InputEventListener _prevPauseEvent;

    public string Title
    {
        get => _titleText.text;
        set => _titleText.text = value;
    }

    protected override void Awake()
    {
        base.Awake();
        _titleText = transform.Find("Header/Text").GetComponent<TextMeshProUGUI>();
        _bodyTrm = (RectTransform)transform.Find("Body");
        _itemParentTrm = _bodyTrm.Find("MainPanel/Items");
        _gridLayout = _itemParentTrm.GetComponent<GridLayoutGroup>();
        _cursor = (RectTransform)transform.Find("Body/MainPanel/Cursor");
        _options = new List<UIDropdownOption>();
    }

    public override void Appear(Transform parentTrm,  Action callback = null)
    {
        base.Appear(parentTrm, callback);

        UIManager.Instance.Interact3DButton = false;
        
        _prevPauseEvent = InputManager.Instance.UIInputReader.OnPauseClickEvent;
        InputManager.Instance.UIInputReader.OnPauseClickEvent = () => Disappear();
        
        _options.Clear();
        _cursorIndex = 0;
        _optionOffset = 0;
        _maxShowOptionCount = 0;
        UpdateCursorPos();
        InputManager.Instance.UIInputReader.OnUpArrowClickEvent += CursorUp;
        InputManager.Instance.UIInputReader.OnDownArrowClickEvent += CursorDown;
        InputManager.Instance.UIInputReader.OnEnterClickEvent += EnterOption;
    }

    public override void Disappear(Action callback = null)
    {
        UIManager.Instance.Interact3DButton = true;
        
        InputManager.Instance.UIInputReader.OnUpArrowClickEvent -= CursorUp;
        InputManager.Instance.UIInputReader.OnDownArrowClickEvent -= CursorDown;
        InputManager.Instance.UIInputReader.OnEnterClickEvent -= EnterOption;
        InputManager.Instance.UIInputReader.OnPauseClickEvent = _prevPauseEvent;
        
        foreach (var option in _options)
        {
            option.Disappear();
        }

        base.Disappear(callback);
    }

    public void AddOption(string optionName, Action callback)
    {
        var option = UIManager.Instance.GenerateUI("DropdownOption", _itemParentTrm) as UIDropdownOption;
        option.Setting(optionName, callback);
        _options.Add(option);

        if (_maxShowOptionCount < 8)
        {
            _maxShowOptionCount++;
            UpdateBodySize();
        }
        else
        {
            UpdateShowOptions();
        }
    }

    private void CursorUp()
    {
        if (_cursorIndex - 1 < 0)
        {
            return;
        }

        _cursorIndex--;
        CalcOptionOffset();
        UpdateCursorPos();
    }

    private void CursorDown()
    {
        if (_cursorIndex + 1 >= _options.Count)
        {
            return;
        }

        _cursorIndex++;
        CalcOptionOffset();
        UpdateCursorPos();
    }

    private void EnterOption()
    {
        _options[_cursorIndex].Invoke();
        Disappear();
    }

    private void UpdateCursorPos()
    {
        var adjustIndex = _cursorIndex - _optionOffset;
        var cursorPosY = -(adjustIndex * _gridLayout.cellSize.y + _gridLayout.spacing.y * adjustIndex);
        _cursor.anchoredPosition3D = new Vector3(0, cursorPosY, 0);
    }

    private void CalcOptionOffset()
    {
        // can offset down
        if (_cursorIndex - _optionOffset < 0)
        {
            _optionOffset--;
            UpdateShowOptions();
        }
        // can offset up
        else if (_cursorIndex - _optionOffset >= _maxShowOptionCount)
        {
            _optionOffset++;
            UpdateShowOptions();
        }
    }

    private void UpdateShowOptions()
    {
        for (var i = 0; i < _options.Count; i++)
        {
            if (i >= _optionOffset && i < _maxShowOptionCount + _optionOffset)
            {
                _options[i].gameObject.SetActive(true);
            }
            else
            {
                _options[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateBodySize()
    {
        var bodySizeY = _maxShowOptionCount * _gridLayout.cellSize.y + _gridLayout.spacing.y * _maxShowOptionCount;
        _bodyTrm.sizeDelta = new Vector2(_bodyTrm.sizeDelta.x, bodySizeY);
    }
}