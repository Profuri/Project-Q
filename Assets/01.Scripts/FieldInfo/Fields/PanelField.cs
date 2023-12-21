using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;

public abstract class PanelField
{
    protected VisualElement _root;
    
    protected VisualTreeAsset _field;
    
    protected VisualElement _fieldRoot;
    protected Label _nameLabel;
    
    
    protected PanelField(VisualElement root,VisualTreeAsset field, FieldInfo info)
    {
        _root = root;
        _field = field;
        
        //루트에 자식으로 들어감 그런데 이 루트 말고 다른 루트에서 가져와여 될 것 같기도 함.
        var container = _field.Instantiate();
        _root.Add(container);

        _fieldRoot = container.Q<VisualElement>("root");
        _nameLabel = _fieldRoot.Q<Label>("Label");

        _nameLabel.text = field.name;
    }
    
    public abstract void Init(FieldInfo info);
}
