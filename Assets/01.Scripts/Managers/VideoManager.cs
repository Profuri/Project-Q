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
    public List<Resolution> ResolutionList { get; private set; }
    private uint _resolutionIndex;
    
    [Header("Full Screen")] 
    [SerializeField] private bool _fullScreen;
    
    [Header("Quality")] 
    [SerializeField] private QualityType _quality;

    public override void Init()
    {
        base.Init();
        ResolutionList = new List<Resolution>();

        foreach (Resolution resolution in Screen.resolutions)
        {
            // 가로세로 비율 계산
            if (ResolutionList.Count <= 0)
            {
                ResolutionList.Add(resolution);
                continue;
            }

            var width = resolution.width;
            var height = resolution.height;
            var lastWidth = ResolutionList.Last().width;
            var lastHeight = ResolutionList.Last().height;

            if (width == lastWidth && height == lastHeight)
            {
                ResolutionList[ResolutionList.Count - 1] = resolution;
            }
            else
            {
                    ResolutionList.Add(resolution);
            }
        }

        LoadToDataManager();
    }

    public override void StartManager()
    {
        SetResolution(ResolutionList.Count - 1);
        SetFullScreen(_fullScreen);
        SetQuality(_quality);
    }

    public void SetResolution(int index)
    {
        var correctIndex = Mathf.Clamp(index, 0, ResolutionList.Count - 1);

        _resolutionIndex = (uint)correctIndex;
        var resolution = ResolutionList[correctIndex];
        var width = resolution.width;
        var height = resolution.height;

        Screen.SetResolution(width, height, Screen.fullScreen);
        CameraManager.Instance.FixedCameraRectWithResolution(width, height);
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
            saveData.ResolutionIndex = _resolutionIndex;
        };
    }

    public Action<SaveData> GetLoadAction()
    {
        return (saveData) =>
        {
            _quality = saveData.DefaultQuality;
            _fullScreen = saveData.DefaultFullScreen;
            _resolutionIndex = saveData.ResolutionIndex;

            SetFullScreen(_fullScreen);
            SetResolution((int)_resolutionIndex);
            SetQuality(_quality);
        };
    }
}