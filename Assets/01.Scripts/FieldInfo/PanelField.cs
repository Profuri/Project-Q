using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;

public abstract class PanelField
{
    protected VisualTreeAsset _field;
    
    protected Label _nameLabel;
    protected VisualElement _root;
    
    
    protected PanelField(VisualTreeAsset field, FieldInfo info)
    {
        _field = field;
        var container = _field.Instantiate();

        _root = container.Q<VisualElement>("root");
        _nameLabel = _root.Q<Label>("Label");

        _nameLabel.text = field.name;
    }
    
    public abstract void Init(FieldInfo info);
}
