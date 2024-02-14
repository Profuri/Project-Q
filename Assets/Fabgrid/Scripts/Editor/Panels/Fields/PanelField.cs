using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using System.Linq;

public abstract class PanelField
{
    private VisualElement _root;
    protected VisualElement _fieldRoot;
    protected Label _label;
    

    protected PanelField(VisualElement root,VisualTreeAsset field, FieldInfo info)
    {
        //추상 클래스 생성자가 실행이 되는지 궁금함
        Debug.Log("CreatePanelField");
        _root = root;
        
        //루트에 자식으로 넣는데 기존에 있는 자식이 사라지고 있는 것 같음
        var container = field.Instantiate();
        _root?.Add(container);

        _fieldRoot = container;
        
        _label = container.Q<Label>("Label");
        if (_label == null)
        {
            Debug.LogError("Can't Find label!");
        }
    }
    
    public abstract void Init(FieldInfo info);
}
