using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class UIButton3D : UIComponent, IClickHandler, IHoverHandler
{
    public UnityEvent OnClickEvent;

    protected Vector3 _originScale;

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
        transform.DOScale(_originScale * 1.1f, 0.2f);
    }

    public virtual void OnHoverCancelHandle()
    {
        transform.DOScale(_originScale, 0.2f);
    }
}