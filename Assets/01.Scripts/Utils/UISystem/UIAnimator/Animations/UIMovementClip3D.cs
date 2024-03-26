using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIMovementClip3D : UIAnimation
{
    public Vector3 fromPosition;
    public Vector3 toPosition;

    public override void Init()
    {
        targetTrm.localPosition = fromPosition;
    }

    public override Tween GetTween()
    {
        return targetTrm.DOLocalMove(toPosition, duration);
    }
}