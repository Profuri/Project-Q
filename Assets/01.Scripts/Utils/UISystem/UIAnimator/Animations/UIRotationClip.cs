using DG.Tweening;
using UnityEngine;

public class UIRotationClip : UIAnimation
{
    public Vector3 fromEuler;
    public Vector3 toEuler;
    
    public override void Init()
    {
        targetTrm.localEulerAngles = fromEuler;
    }

    public override Tween GetTween()
    {
        return targetTrm.DOLocalRotate(toEuler, duration);
    }
}