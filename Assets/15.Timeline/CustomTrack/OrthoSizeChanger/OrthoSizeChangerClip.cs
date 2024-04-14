using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class OrthoSizeChangerClip : PlayableAsset, ITimelineClipAsset
{
    public OrthoSizeChangerBehaviour template = new OrthoSizeChangerBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<OrthoSizeChangerBehaviour>.Create (graph, template);
        return playable;
    }
}
