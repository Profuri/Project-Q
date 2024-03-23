using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public enum QualityType
{
    Low = 0,
    Medium = 1,
    High = 2
}

public class VideoManager : BaseManager<VideoManager>
{
    [Header("Resolution")] 
    private Resolution[] _resolutions;
    
    [Header("Full Screen")] 
    [SerializeField] private bool _defalutFullScreen;
    
    [Header("Quality")] 
    [SerializeField] private QualityType _defalutQuality;

    public override void Init()
    {
        base.Init();
        _resolutions = Screen.resolutions;
    }

    public override void StartManager()
    {
        SetResolution(_resolutions.Length - 1);
        SetFullScreen(_defalutFullScreen);
        SetQuality(_defalutQuality);
    }

    public void SetResolution(int index)
    {
        var correctIndex = Mathf.Clamp(index, 0, _resolutions.Length - 1);

        var resolution = _resolutions[correctIndex];
        var width = resolution.width;
        var height = resolution.height;

        Screen.SetResolution(width, height, Screen.fullScreen);
    }
    
    public void SetFullScreen(bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
    }

    public void SetQuality(QualityType type)
    {
        QualitySettings.SetQualityLevel((int)type);
    }

}