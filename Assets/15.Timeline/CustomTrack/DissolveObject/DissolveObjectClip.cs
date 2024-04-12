using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DissolveObjectClip : PlayableAsset, ITimelineClipAsset
{
    public DissolveObjectBehaviour template = new DissolveObjectBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DissolveObjectBehaviour>.Create (graph, template);
        return playable;
    }
}
