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
        seq.SetAutoKill(false);
        seq.SetUpdate(true);

        _prevStartTime = 0f;
        _prevEndTime = 0f;
        
        foreach (var clip in clips)
        {
            if (clip.joinPrevAnimation)
            {
                clip.Init();
                seq.Insert(_prevStartTime, clip.GetTween());
            }
            else
            {
                seq.InsertCallback(_prevEndTime, () => clip.Init());
                seq.Insert(_prevEndTime, clip.GetTween());
                _prevStartTime = _prevEndTime;
            }
            
            _prevEndTime = _prevStartTime + clip.duration;
        }

        seq.OnComplete(() => callBack?.Invoke());
    }
}