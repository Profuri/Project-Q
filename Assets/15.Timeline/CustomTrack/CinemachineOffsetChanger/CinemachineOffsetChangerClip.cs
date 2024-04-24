using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CinemachineOffsetChangerClip : PlayableAsset, ITimelineClipAsset
{
    public CinemachineOffsetChangerBehaviour template = new CinemachineOffsetChangerBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CinemachineOffsetChangerBehaviour>.Create (graph, template);
        CinemachineOffsetChangerBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
