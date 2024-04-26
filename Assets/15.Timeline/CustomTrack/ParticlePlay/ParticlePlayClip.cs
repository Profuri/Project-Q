using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class ParticlePlayClip : PlayableAsset, ITimelineClipAsset
{
    public ParticlePlayBehaviour template = new ParticlePlayBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ParticlePlayBehaviour>.Create (graph, template);
        return playable;
    }
}
