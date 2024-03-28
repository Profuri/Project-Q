using System;
using System.Collections.Generic;
using System.Linq;
using ManagingSystem;
using UnityEngine;

public enum QualityType
{
    Low = 0,
    Medium = 1,
    High = 2
}

public class VideoManager : BaseManager<VideoManager>, IProvideSave,IProvideLoad
{
    public Resolution[] Resolutions { get; private set; }
    private uint _resolutionIndex;
    
    [Header("Full Screen")] 
    [SerializeField] private bool _fullScreen;
    
    [Header("Quality")] 
    [SerializeField] private QualityType _quality;

    public override void Init()
    {
        base.Init();
        Resolutions = Screen.resolutions;

        LoadToDataManager();
    }

    public override void StartManager()
    {
        SetResolution(Resolutions.Length - 1);
        SetFullScreen(_fullScreen);
        SetQuality(_quality);

    }

    public void SetResolution(int index)
    {
        var correctIndex = Mathf.Clamp(index, 0, Resolutions.Length - 1);

        _resolutionIndex = (uint)correctIndex;
        var resolution = Resolutions[correctIndex];
        var width = resolution.width;
        var height = resolution.height;

        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    public void SetFullScreen(bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
        _fullScreen = fullScreen;
    }

    public void SetQuality(QualityType type)
    {
        QualitySettings.SetQualityLevel((int)type);
        _quality = type;
    }

    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(this,this);
    }

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            saveData.DefaultQuality = _quality;
            saveData.DefaultFullScreen = _fullScreen;
            saveData.resolutionIndex = _resolutionIndex;
        };
    }

    public Action<SaveData> GetLoadAction()
    {
        return (saveData) =>
        {
            _quality = saveData.DefaultQuality;
            _fullScreen = saveData.DefaultFullScreen;
            _resolutionIndex = saveData.resolutionIndex;

            SetFullScreen(_fullScreen);
            SetResolution((int)_resolutionIndex);
            SetQuality(_quality);
        };
    }
}