using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1f, 0f, 0.125886f)]
[TrackClipType(typeof(DissolveObjectClip))]
[TrackBindingType(typeof(GameObject))]
public class DissolveObjectTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<DissolveObjectMixerBehaviour>.Create (graph, inputCount);
    }
}
