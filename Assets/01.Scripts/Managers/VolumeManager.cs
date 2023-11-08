using System.Collections;
using ManagingSystem;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeManager : BaseManager<VolumeManager>
{
    [SerializeField] private Volume _mainVolume;
    [SerializeField] private Volume _highlightVolume;
    
    public override void StartManager()
    {
        _mainVolume.weight = 1;
        _highlightVolume.weight = 0;
    }

    public override void UpdateManager()
    {
        // Do Nothing
    }

    public void Highlight(float time)
    {
        StartCoroutine(HighlightRoutine(time));
    }

    private IEnumerator HighlightRoutine(float time, float changePercent = 0.5f)
    {
        var curTime = 0f;
        var percent = 0f;

        while (percent <= 1)
        {
            curTime += Time.deltaTime;
            percent = curTime / time;

            if (percent < changePercent)
            {
                _mainVolume.weight = 1f - percent * 2f;
                _highlightVolume.weight = percent * 2f;
            }
            else
            {
                _mainVolume.weight = (percent - changePercent) * 2f;
                _highlightVolume.weight = 1 - (percent - changePercent) * 2f;
            }
            yield return null;
        }
    }
}
