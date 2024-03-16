using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIMovementClip : UIAnimation
{
    public Vector2 fromPosition;
    public Vector2 toPosition;

    public override void Init()
    {
        target.anchoredPosition = fromPosition;
    }

    public override Tween GetTween()
    {
        return target.DOAnchorPos(toPosition, duration).SetUpdate(true);
    }
}