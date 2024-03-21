using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIScalingClip : UIAnimation
{
    public Vector3 fromScale;
    public Vector3 toScale;
    
    public override void Init()
    {
        targetTrm.localScale = fromScale;
    }

    public override Tween GetTween()
    {
        return targetTrm.DOScale(toScale, duration);
    }
}