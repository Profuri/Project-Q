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
        return ScriptPlayable<OrthoSizeChangerMixerBehaviour>.Create (graph, inputCount);
    }
}
