using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Fabgrid;
using UnityEditor.ShaderKeywordFilter;

public class DropdownField : PanelField
{
    private EnumField _enumField;
    public DropdownField(VisualElement root,VisualTreeAsset field, FieldInfo info) : base(root,field, info)
    {
        _enumField = _fieldRoot.Q<EnumField>();
        //두 번째에 있는 Element를 찾아옴.
        Init(info);
    }
    
    public override void Init(FieldInfo info)
    {
        Type type = info.FieldType;
        var enumValue = Activator.CreateInstance(type);
        if (enumValue is Enum)
        {
            Enum enumField = enumValue as Enum;
            _enumField.Init(enumField);
        }

        string labelName = info.Name;
        _enumField.label = labelName;
        //이거 필드 옆에 글자 추가하는 함수임
        // foreach (Enum value in Enum.GetValues(type))
        // {
        //     UnityEngine.UIElements.TextElement textElement = new TextElement();
        //     textElement.text = value.ToString();
        //     _enumField.Add(textElement);
        //     //_enumField.
        // }
    }
}
