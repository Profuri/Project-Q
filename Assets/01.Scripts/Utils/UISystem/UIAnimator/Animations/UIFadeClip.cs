using DG.Tweening;
using UnityEngine;

public class UIFadeClip : UIAnimation
{
    public float fromAlpha;
    public float toAlpha;

    private CanvasGroup _canvasGroup;

    public override void Init()
    {
        _canvasGroup = targetTrm.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = fromAlpha;
    }

    public override Tween GetTween()
    {
        return DOTween.To(() => _canvasGroup.alpha, alpha => _canvasGroup.alpha = alpha, toAlpha, duration);
    }
}