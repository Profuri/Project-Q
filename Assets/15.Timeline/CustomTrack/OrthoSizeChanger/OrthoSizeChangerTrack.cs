using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.1792453f, 0.1792453f, 0.1792453f)]
[TrackClipType(typeof(OrthoSizeChangerClip))]
[TrackBindingType(typeof(CinemachineVirtualCamera))]
public class OrthoSizeChangerTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        foreach (var clip in GetClips())
        {
            var customClip = clip.asset as OrthoSizeChangerClip;
            if (customClip != null)
            {
                customClip.ClipStart = clip.start;
                customClip.ClipEnd = clip.end;

                if (clip.easeInDuration > 0f)
                {
                    customClip.ClipStartWeight = clip.mixInCurve.Evaluate(0f);
                    customClip.ClipEndWeight = clip.mixInCurve.Evaluate(1f);
                }
                else if (clip.easeOutDuration > 0f)
                {
                    customClip.ClipStartWeight = clip.mixOutCurve.Evaluate(0f);
                    customClip.ClipEndWeight = clip.mixOutCurve.Evaluate(1f);
                }
            }
        }
        return ScriptPlayable<OrthoSizeChangerMixerBehaviour>.Create (graph, inputCount);
    }
}
