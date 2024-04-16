using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.1556604f, 0.5778302f, 1f)]
[TrackClipType(typeof(CameraShakeClip))]
[TrackBindingType(typeof(CinemachineVirtualCamera))]
public class CameraShakeTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CameraShakeMixerBehaviour>.Create (graph, inputCount);
    }
}
