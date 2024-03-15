using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIAnimator
{
    public readonly List<UIAnimation> Clips = new List<UIAnimation>();

    public void Play()
    {
        var seq = DOTween.Sequence();
        seq.SetAutoKill(false);
        seq.SetUpdate(true);
        
        foreach (var clip in Clips)
        {
            if (clip.joinPrevAnimation)
            {
                seq.Join(clip.GetAnimationTween());
            }
            else
            {
                seq.Append(clip.GetAnimationTween());
            }
        }
        
        seq.Play();
    }
}