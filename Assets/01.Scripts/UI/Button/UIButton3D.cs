using DG.Tweening;
using TinyGiantStudio.Text;
using UnityEngine;
using UnityEngine.Events;

public class UIButton3D : UIComponent, IClickHandler, IHoverHandler
{
    public UnityEvent<UIButton3D> OnClickEvent;

    protected Vector3 _originScale;
    protected Modular3DText _3dText;

    public string Text
    {
        get => _3dText.Text;
        set => _3dText.Text = value;
    }

    protected override void Awake()
    {
        base.Awake();
        _originScale = transform.localScale;
        _3dText = GetComponentInChildren<Modular3DText>();
    }

    public virtual void OnClickHandle()
    {
        OnClickEvent?.Invoke(this);
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