using DG.Tweening;
using TinyGiantStudio.Text;
using UnityEngine;
using UnityEngine.Events;

public class UIButton3D : UIComponent, IClickHandler, IHoverHandler
{
    public UnityEvent OnClickEvent;

    protected Vector3 OriginScale;
    protected Modular3DText _3dText;

    public string Text
    {
        get => _3dText.Text;
        set => _3dText.Text = value;
    }

    protected override void Awake()
    {
        base.Awake();
        OriginScale = transform.localScale;
        _3dText = GetComponentInChildren<Modular3DText>();
    }

    public virtual void OnClickHandle()
    {
        OnClickEvent?.Invoke();
    }

    public virtual void OnHoverHandle()
    {
        transform.DOScale(OriginScale * 1.2f, 0.2f).SetUpdate(true);
    }

    public virtual void OnHoverCancelHandle()
    {
        transform.DOScale(OriginScale, 0.2f).SetUpdate(true);
    }
}