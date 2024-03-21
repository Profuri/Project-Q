using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class UIButton3D : UIComponent, IClickHandler, IHoverHandler
{
    public UnityEvent OnClickEvent;

    private bool _isHover;
    private Vector3 _originScale;

    protected override void Awake()
    {
        base.Awake();
        _originScale = transform.localScale;
    }

    public virtual void OnClickHandle()
    {
        OnClickEvent?.Invoke();
    }

    public virtual void OnHoverHandle()
    {
        _isHover = true;
        transform.DOScale(_originScale * 1.2f, 0.2f);
    }

    public void OnHoverCancelHandle()
    {
        _isHover = false;
        transform.DOScale(_originScale, 0.2f);
    }
}