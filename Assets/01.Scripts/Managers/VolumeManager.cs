using System.Collections;
using ManagingSystem;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeManager : BaseManager<VolumeManager>
{
    [SerializeField] private Volume _mainVolume;
    [SerializeField] private Volume _highlightVolume;
    [SerializeField] private Volume _axisControlVolume;
    
    public override void StartManager()
    {
        _mainVolume.weight = 1;
        _highlightVolume.weight = 0;
        _axisControlVolume.weight = 0;
    }

    public override void UpdateManager()
    {
        // Do Nothing
    }

    public void SetAxisControlVolume(bool enable, float time)
    {
        if (enable)
        {
            StartCoroutine(VolumeChangeRoutine(_mainVolume, _axisControlVolume, time));
        }
        else
        {
            StartCoroutine(VolumeChangeRoutine(_axisControlVolume, _mainVolume, time));
        }
    }

    public void Highlight(float time)
    {
        StartCoroutine(VolumeChangeRoutine(_mainVolume, _highlightVolume, time, true));
    }

    private IEnumerator VolumeChangeRoutine(Volume prev, Volume next, float time, bool isReturn = false, float returnPoint = 0.5f)
    {
        var curTime = 0f;
        var percent = 0f;

        while (percent <= 1)
        {
            curTime += Time.deltaTime;
            percent = curTime / time;

            if (isReturn)
            {
                if (percent < returnPoint)
                {
                    prev.weight = 1f - percent * 2f;
                    next.weight = percent * 2f;
                }
                else
                {
                    prev.weight = (percent - returnPoint) * 2f;
                    next.weight = 1 - (percent - returnPoint) * 2f;
                }
            }
            else
            {
                prev.weight = 1f - percent;
                next.weight = percent;
            }

            yield return null;
        }
    }
}
