using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0f, 0.5f, 1f)]
[TrackClipType(typeof(CinemachineOffsetChangerClip))]
[TrackBindingType(typeof(CinemachineVirtualCamera))]
public class CinemachineOffsetChangerTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CinemachineOffsetChangerMixerBehaviour>.Create (graph, inputCount);
    }
}
