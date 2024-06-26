using DG.Tweening;
using UnityEngine.UI;

[System.Serializable]
public class UILayoutSpacingClip : UIAnimation
{
    public float fromSpacing;
    public float targetSpacing;

    private HorizontalLayoutGroup _layoutGroup;
    
    public override void Init()
    {
        _layoutGroup = targetTrm.GetComponent<HorizontalLayoutGroup>();
        _layoutGroup.spacing = fromSpacing;
    }

    public override Tween GetTween()
    {
        return DOTween.To(() => _layoutGroup.spacing, spacing => _layoutGroup.spacing = spacing, targetSpacing, duration);
    }
}