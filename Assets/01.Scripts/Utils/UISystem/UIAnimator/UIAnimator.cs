using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIAnimator
{
    public readonly List<UIAnimation> Clips = new List<UIAnimation>();

    private Sequence _sequence;

    public UIAnimator()
    {
        _sequence = DOTween.Sequence();
        _sequence.SetAutoKill(false);
        _sequence.SetUpdate(true);
    }

    public void Play()
    {
        _sequence.Complete();
        foreach (var clip in Clips)
        {
            if (clip.joinPrevAnimation)
            {
                _sequence.Join(clip.GetAnimationTween());
            }
            else
            {
                _sequence.Append(clip.GetAnimationTween());
            }
        }
        _sequence.OnComplete(Stop);
        _sequence.Play();
    }

    public void Stop()
    {
        _sequence.Pause();
    }
}