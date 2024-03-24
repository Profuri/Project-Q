using System;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Vector2 _padding;
    [SerializeField] private LayerMask _clickableMask;

    public UIComponent TopComponent => _componentStack.Peek();
    
    private Stack<UIComponent> _componentStack;

    private UIButton3D _handleUI;
    private IClickUpHandler _heldHandler;

    private bool _interact3DButton;
    public bool Interact3DButton
    {
        get => _interact3DButton;
        set
        {
            _interact3DButton = value;
            if (!_interact3DButton)
            {
                if (_handleUI)
                {
                    _handleUI.OnHoverCancelHandle();
                    _handleUI = null;
                }

                if (_heldHandler != null)
                {
                    _heldHandler.OnClickUpHandle();
                    _heldHandler = null;
                }
            }
        }
    }

    public override void Init()
    {
        base.Init();
        _componentStack = new Stack<UIComponent>();
    }

    public override void StartManager()
    {
        InputManager.Instance.InputReader.OnLeftClickEvent += OnUIClickHandle;
        InputManager.Instance.InputReader.OnLeftClickUpEvent += OnUIClickUpHandle;
        Interact3DButton = true;
    }

    private void Update()
    {
        if (Interact3DButton)
        {
            FindUIHandler();
        }
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
            if (_handleUI.TryGetComponent<IClickUpHandler>(out var clickUpHandler))
            {
                _heldHandler = clickUpHandler;
            }
            
            clickable.OnClickHandle();
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

    public void RemoveTopUI()
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
        RemoveTopUI();
        if (TopComponent is not null)
        {
            GenerateUI(TopComponent, TopComponent.ParentTrm);
        }
    }

    private void ClearPanel()
    {
        while (_componentStack.Count > 0)
        {
            RemoveTopUI();
        }
    }
    
    public Vector3 AdjustUIRectPosition(Vector3 position, Rect rect)
    {
        var canvasRect = _mainCanvas.pixelRect;
        var adjustPos = position;

        var xDiff = adjustPos.x + rect.width - (canvasRect.width - _padding.x);
        var yDiff = adjustPos.y - rect.height - _padding.y;

        if (xDiff > 0)
        {
            adjustPos.x -= xDiff;
        }
        
        if (yDiff < 0)
        {
            adjustPos.y -= yDiff;
        }
        
        return adjustPos;
    }
}