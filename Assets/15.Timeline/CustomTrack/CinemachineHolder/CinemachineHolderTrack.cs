using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0f, 0.5f, 1f)]
[TrackClipType(typeof(CinemachineHolderClip))]
[TrackBindingType(typeof(CinemachineVirtualCamera))]
public class CinemachineHolderTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CinemachineHolderMixerBehaviour>.Create (graph, inputCount);
    }
}
