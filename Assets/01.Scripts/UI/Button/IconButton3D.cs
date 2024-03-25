using DG.Tweening;
using UnityEngine;

public class IconButton3D : UIButton3D
{
    private Transform _iconTrm;

    protected override void Awake()
    {
        base.Awake();
        _iconTrm = transform.Find("Icon");
        _originScale = _iconTrm.localScale;
    }

    public override void OnHoverHandle()
    {
        _iconTrm.DOScale(_originScale * 1.1f, 0.2f).SetUpdate(true);
    }

    public override void OnHoverCancelHandle()
    {
        _iconTrm.DOScale(_originScale, 0.2f).SetUpdate(true);
    }
}