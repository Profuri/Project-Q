using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIAnimation : ScriptableObject
{
    public float duration;
    public Transform target;
    public bool joinPrevAnimation;

    public virtual void Init() { }
    public virtual Tween GetTween() { return null; }
}