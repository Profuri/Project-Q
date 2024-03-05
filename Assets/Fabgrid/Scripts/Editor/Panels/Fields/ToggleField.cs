using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using Fabgrid;

public class ToggleField : PanelField
{
    private UnityEngine.UIElements.Toggle _toggle;
    
    public ToggleField(VisualElement root,VisualTreeAsset field,FieldInfo info) : base(root,field,info)
    {
        _toggle = _fieldRoot.Q<Toggle>();
        //base(field, info);        
        //만약 안된다면 부무 생성자가 실행이 안 되는지 확인.
        Init(info);
    }

    public override void Init(FieldInfo info)
    {
        string labelName = info.Name;
        _toggle.label = labelName;
    }
}