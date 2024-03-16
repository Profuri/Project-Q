using System.Collections.Generic;
using DG.Tweening;

[System.Serializable]
public class UIAnimator
{
    public List<UIAnimation> clips = new List<UIAnimation>();

    public void Play()
    {
        var seq = DOTween.Sequence();
        seq.SetAutoKill(false);
        seq.SetUpdate(true);
        
        foreach (var clip in clips)
        {
            seq.AppendCallback(() => clip.Init());
            
            if (clip.joinPrevAnimation)
            {
                seq.Join(clip.GetTween());
            }
            else
            {
                seq.Append(clip.GetTween());
            }
        }
    }
}