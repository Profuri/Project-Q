using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CinemachineHolderClip : PlayableAsset, ITimelineClipAsset
{
    public CinemachineHolderBehaviour template = new CinemachineHolderBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CinemachineHolderBehaviour>.Create (graph, template);
        return playable;
    }
}
