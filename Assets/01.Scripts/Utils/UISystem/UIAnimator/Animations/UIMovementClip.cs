using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIMovementClip : UIAnimation
{
    public Vector2 fromPosition;
    public Vector2 toPosition;
    
    public override Tween GetAnimationTween()
    {
        target.anchoredPosition = fromPosition;
        return target.DOMove(toPosition, duration);
    }
}