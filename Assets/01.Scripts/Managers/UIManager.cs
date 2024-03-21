using System;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private LayerMask _clickableMask;

    public UIComponent TopComponent => _componentStack.Peek();
    
    private Stack<UIComponent> _componentStack;

    private UIButton3D _handleUI;
    private IClickUpHandler _heldHandler;

    public override void Init()
    {
        base.Init();
        _componentStack = new Stack<UIComponent>();
    }

    public override void StartManager()
    {
        InputManager.Instance.InputReader.OnLeftClickEvent += OnUIClickHandle;
        InputManager.Instance.InputReader.OnLeftClickUpEvent += OnUIClickUpHandle;
    }

    private void Update()
    {
        FindUIHandler();
    }

    private void FindUIHandler()
    {
        if (_heldHandler is not null)
        {
            return;
        }
        
        var mouseScreenPoint = InputManager.Instance.InputReader.mouseScreenPoint;
        var ray = CameraManager.Instance.MainCam.ScreenPointToRay(mouseScreenPoint);
        var isHit = Physics.Raycast(ray, out var hit, Mathf.Infinity, _clickableMask);

        if (!isHit)
        {
            if (_handleUI is not null)
            {
                _handleUI.OnHoverCancelHandle();
                _handleUI = null;
            }
            return;
        }

        if (hit.collider.TryGetComponent<UIButton3D>(out var component))
        {
            if (_handleUI is not null)
            {
                _handleUI.OnHoverCancelHandle();
            }
            
            _handleUI = component;
            _handleUI.OnHoverHandle();
        }
    }

    private void OnUIClickHandle(Vector2 mouseScreenPoint)
    {
        if (_handleUI == null)
        {
            return;
        }
        
        if (_handleUI.TryGetComponent<IClickHandler>(out var clickable))
        { 
            clickable.OnClickHandle();

            if (_handleUI.TryGetComponent<IClickUpHandler>(out var clickUpHandler))
            {
                _heldHandler = clickUpHandler;
            }
        }
    }

    private void OnUIClickUpHandle(Vector2 mouseScreenPoint)
    {
        if (_heldHandler is not null)
        {
            _heldHandler.OnClickUpHandle();
            _heldHandler = null;
        }
    }

    public UIComponent GenerateUI(string key, Transform parent = null)
    {
        if (parent is null)
        {
            parent = _mainCanvas.transform;
        }

        var component = SceneControlManager.Instance.AddObject(key) as UIComponent;
        return ComponentSetting(component, parent);
    }
    
    public UIComponent GenerateUI(UIComponent component, Transform parent = null)
    {
        if (parent is null)
        {
            parent = _mainCanvas.transform;
        }
        
        return ComponentSetting(component, parent);
    }

    private UIComponent ComponentSetting(UIComponent component, Transform parent)
    {
        if (component != null)
        {
            if (component.componentType == UIComponentType.Screen)
            {
                ClearPanel();
            }

            if (component.componentType is UIComponentType.Screen or UIComponentType.Popup)
            {
                _componentStack.Push(component);
            }

            component.Appear(parent);
        }

        return component;
    }

    public void RemoveUI()
    {
        if (_componentStack.Count <= 0)
        {
            return;
        }
        
        var top = _componentStack.Pop();
        top.Disappear();
    }

    public void ReturnUI()
    {
        RemoveUI();
        if (TopComponent is not null)
        {
            GenerateUI(TopComponent, TopComponent.ParentTrm);
        }
    }

    private void ClearPanel()
    {
        while (_componentStack.Count > 0)
        {
            RemoveUI();
        }
    }
}