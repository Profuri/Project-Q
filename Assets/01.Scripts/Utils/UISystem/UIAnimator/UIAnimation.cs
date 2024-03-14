using DG.Tweening;
using UnityEngine;

[System.Serializable]
public abstract class UIAnimation
{
    public float duration;
    public RectTransform target;
    public bool joinPrevAnimation;

    public abstract Tween GetAnimationTween();
}