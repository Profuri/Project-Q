using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class UIAnimator
{
    public List<UIAnimation> clips = new List<UIAnimation>();
    private float _prevStartTime;
    private float _prevEndTime;

    public bool IsPlay {get; private set;} = false;

    public void Init(UIComponent component)
    {
        foreach (var clip in clips)
        {
            clip.SetTargetTrm(component.transform);
        }
    }

    public void Play(Action callBack = null)
    {
        var seq = DOTween.Sequence();
        IsPlay = true;
        seq.SetAutoKill(false);
        seq.SetUpdate(true);

        _prevStartTime = 0f;
        _prevEndTime = 0f;
        
        foreach (var clip in clips)
        {
            if(!clip.joinPrevAnimation)
            {
                _prevStartTime = _prevEndTime;
            }

            seq.InsertCallback(_prevStartTime, clip.Init).Join(clip.GetTween().SetEase(clip.ease));
            _prevEndTime = _prevStartTime + clip.duration;
        }

        seq.OnComplete(() => 
        {
            IsPlay = false;
            callBack?.Invoke();
        });
    }
}