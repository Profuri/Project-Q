using System;
using System.Collections;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeManager : BaseManager<VolumeManager>
{
    [SerializeField] private VolumeData _volumeData;

    private Dictionary<VolumeType, Volume> _volumes;
    private Volume _currentVolume;

    public override void Init()
    {
        base.Init();

        _volumes = new Dictionary<VolumeType, Volume>();
        
        var volumeParent = new GameObject("Volumes").transform;
        volumeParent.SetParent(transform);

        foreach (VolumeType type in Enum.GetValues(typeof(VolumeType)))
        {
            var volumeObjectName = $"{type}Volume";
            
            var newVolumeObject = new GameObject(volumeObjectName);
            newVolumeObject.transform.SetParent(volumeParent);
            
            var volumeCompo = newVolumeObject.AddComponent<Volume>();
            var profile = _volumeData.GetVolumeProfile(type);

            volumeCompo.weight = 0f;
            volumeCompo.profile = profile;
            
            _volumes[type] = volumeCompo;
        }
    }

    public override void StartManager()
    {
        SetVolume(VolumeType.Default, 0f);
    }

    public void SetVolume(VolumeType type, float time, bool isReturnOrigin = false, float returningPoint = 0.5f)
    {
        var nextVolume = _volumes[type];
        StartSafeCoroutine("VolumeChangeRoutine", VolumeChangeRoutine(_currentVolume, nextVolume, time, isReturnOrigin, returningPoint));
        if (!isReturnOrigin)
        {
            _currentVolume = nextVolume;
        }
    }

    private IEnumerator VolumeChangeRoutine(Volume prev, Volume next, float time, bool isReturnOrigin, float returningPoint)
    {
        var curTime = 0f;
        var percent = 0f;

        while (percent <= 1)
        {
            curTime += Time.deltaTime;
            percent = curTime / time;
            var clampedPercent = Mathf.Clamp(percent, 0f, 01f);

            if (isReturnOrigin)
            {
                if (clampedPercent < returningPoint)
                {
                    if (prev)
                    {
                        prev.weight = 1f - clampedPercent / returningPoint;
                    }
                    next.weight = clampedPercent / returningPoint;
                }
                else
                {
                    if (prev)
                    {
                        prev.weight = (clampedPercent - returningPoint) * 2f;
                    }
                    next.weight = 1 - (clampedPercent - returningPoint) * 2f;
                }
            }
            else
            {
                if (prev)
                {
                    prev.weight = 1f - clampedPercent;
                }
                next.weight = clampedPercent;
            }

            yield return null;
        }
    }
}
