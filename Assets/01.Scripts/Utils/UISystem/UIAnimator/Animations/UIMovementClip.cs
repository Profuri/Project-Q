using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIMovementClip : UIAnimation
{
    public Vector2 fromPosition;
    public Vector2 toPosition;

    public override void Init()
    {
        targetTrm.localPosition = fromPosition;
    }

    public override Tween GetTween()
    {
        return targetTrm.DOLocalMove(toPosition, duration);
    }
}