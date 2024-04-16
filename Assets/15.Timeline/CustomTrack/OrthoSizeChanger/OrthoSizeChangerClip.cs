using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class OrthoSizeChangerClip : PlayableAsset, ITimelineClipAsset
{
    public OrthoSizeChangerBehaviour template = new OrthoSizeChangerBehaviour ();

    public double ClipStart { get; set; }
    public double ClipEnd { get; set; }
    public float ClipStartWeight { get; set; }
    public float ClipEndWeight { get; set; }
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<OrthoSizeChangerBehaviour>.Create (graph, template);
        var behaviour = playable.GetBehaviour();

        behaviour.start = ClipStart;
        behaviour.end = ClipEnd;
        behaviour.startWeight = ClipStartWeight;
        behaviour.endWeight = ClipEndWeight;
        
        return playable;
    }
}
