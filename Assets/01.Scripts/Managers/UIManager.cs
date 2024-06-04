using System;
using ManagingSystem;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    [Header("For 2D UI")]
    [SerializeField] private Canvas _mainCanvas2D;
    public Canvas MainCanvas2D => _mainCanvas2D;
    
    [SerializeField] private Vector2 _padding;
    
    [Header("For 3D UI")]
    [SerializeField] private Transform _mainCanvas3D;
    
    [Space(10)]
    [SerializeField] private LayerMask _clickableMask;

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

    public override void StartManager()
    {
        InputManager.Instance.UIInputReader.OnLeftClickEvent += OnUIClickHandle;
        InputManager.Instance.UIInputReader.OnLeftClickUpEvent += OnUIClickUpHandle;
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
        
        var mouseScreenPoint = InputManager.Instance.UIInputReader.mouseScreenPoint;
        var ray = CameraManager.Instance.MainCam.ScreenPointToRay(mouseScreenPoint);
        var isHit = Physics.Raycast(ray, out var hit, Mathf.Infinity, _clickableMask);

        if (!isHit)
        {
            if (_handleUI != null)
            {
                _handleUI.OnHoverCancelHandle();
                _handleUI = null;
            }
            return;
        }

        if (hit.collider.TryGetComponent<UIButton3D>(out var component))
        {
            if (_handleUI != null)
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

    public UIComponent GenerateUI(string key, Transform parent = null, Action callback = null)
    {
        var component = SceneControlManager.Instance.AddObject(key) as UIComponent;
        
        if (parent is null)
        {
            parent = component.transform is RectTransform ? _mainCanvas2D.transform : _mainCanvas3D;
        }

        component.Appear(parent, callback);
        return component;
    }

    public Vector3 AdjustUIRectPosition(Vector3 position, Rect rect)
    {
        var canvasRect = ((RectTransform)_mainCanvas2D.transform).rect;
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