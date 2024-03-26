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

public class VideoManager : BaseManager<VideoManager>, IDataProvidable
{
    public Resolution[] Resolutions { get; private set; }
    
    [Header("Full Screen")] 
    [SerializeField] private bool _defaultFullScreen;
    
    [Header("Quality")] 
    [SerializeField] private QualityType _defaultQuality;

    public override void Init()
    {
        base.Init();
        Resolutions = Screen.resolutions;
    }

    public override void StartManager()
    {
        SetResolution(Resolutions.Length - 1);
        SetFullScreen(_defaultFullScreen);
        SetQuality(_defaultQuality);

        LoadToDataManager();
    }

    public void SetResolution(int index)
    {
        var correctIndex = Mathf.Clamp(index, 0, Resolutions.Length - 1);

        var resolution = Resolutions[correctIndex];
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

    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(this);
    }

    public Action<SaveData> GetProvideAction()
    {
        return (saveData) =>
        {
            saveData.DefaultQuality = _defaultQuality;
            saveData.DefaultFullScreen = _defaultFullScreen;
        };
    }

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            _defaultQuality = saveData.DefaultQuality;
            _defaultFullScreen = saveData.DefaultFullScreen;
        };
    }
}