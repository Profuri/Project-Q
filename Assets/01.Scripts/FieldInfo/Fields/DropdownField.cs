using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DropdownField : PanelField
{
    private VisualElement _registerParentElem;

    private VisualElement _textElement;    
    public DropdownField(VisualElement root,VisualTreeAsset field, FieldInfo info) : base(root,field, info)
    {
        //두 번째에 있는 Element를 찾아옴.
        _registerParentElem = _fieldRoot.Q<VisualElement>("VisualElement");
        _textElement = _registerParentElem.Q<VisualElement>("TextElement");

        Init(info);
    }
    
    public override void Init(FieldInfo info)
    {
        Type type = info.GetType();
        foreach (Enum value in Enum.GetValues(type))
        {
            UnityEngine.UIElements.TextElement textElement = new TextElement();
            textElement.text = value.ToString();
            //textElement.
            _registerParentElem.Add(textElement);            
        }
    }
}
