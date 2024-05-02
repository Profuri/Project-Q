using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "SO/VolumeData")]
public class VolumeData : ScriptableObject
{
    public List<VolumeSet> data = new List<VolumeSet>();
    private Dictionary<VolumeType, VolumeProfile> _dataDiction;

    private void OnEnable()
    {
        _dataDiction = new Dictionary<VolumeType, VolumeProfile>();
        foreach (var set in data)
        {
            _dataDiction[set.type] = set.profile;
        }
    }

    public VolumeProfile GetVolumeProfile(VolumeType type)
    {
        return _dataDiction[type];
    }
}