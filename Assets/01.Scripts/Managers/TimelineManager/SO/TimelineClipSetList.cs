using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "SO/TimelineClipSetList")]
public class TimelineClipSetList : ScriptableObject
{
    public List<TimelineClipSet> list;

    public TimelineAsset GetAsset(TimelineType type)
    {
        foreach (var set in list)
        {
            if (set.type == type)
            {
                return set.asset;
            }
        }

        return null;
    }
}