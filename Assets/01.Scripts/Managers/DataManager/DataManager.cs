using System;
using System.Collections.Generic;
using System.Linq;
using Singleton;
using UnityEngine;
using ManagingSystem;

public class SaveData
{
    public SaveData()
    {
        ChapterClearDictionary = new SerializableDictionary<ChapterType, bool>();
        ChapterTimelineDictionary = new SerializableDictionary<ChapterType, bool>();

        foreach (ChapterType chapterType in Enum.GetValues(typeof(ChapterType)))
        {
            ChapterClearDictionary.Add(chapterType, false);
            ChapterTimelineDictionary.Add(chapterType, false);
        }
        
        VolumeDictionary = new SerializableDictionary<EAUDIO_MIXER, float>();

        ResolutionIndex = (uint)Screen.resolutions.Length - 1;
        DefaultQuality = QualityType.High;
    }

    public SerializableDictionary<ChapterType, bool> ChapterClearDictionary;
    public SerializableDictionary<ChapterType, bool> ChapterTimelineDictionary;
    public SerializableDictionary<EAUDIO_MIXER, float> VolumeDictionary;

    public bool DefaultFullScreen;
    public QualityType DefaultQuality;
    public uint ResolutionIndex;

    public string KeyBinding;
}


public interface IProvideSave
{
    public Action<SaveData> GetSaveAction();
}

public interface IProvideLoad
{
    public Action<SaveData> GetLoadAction();
}


public class DataManager : BaseManager<DataManager>
{
    [Header("SaveSettings")]
    [SerializeField] private string _fileName;
    [SerializeField] private bool _isEncrypt;
    [SerializeField] private bool _isBase64;

    private FileDataHandler _fileDataHandler;

    //얘는 저장하는 이벤트가 달린 딕셔너리
    private readonly Dictionary<IProvideSave, Action<SaveData>> _dataSaveDictionary = new Dictionary<IProvideSave, Action<SaveData>>();

    //얘는 저장된 데이터를 바탕으로 게임에 설정하는 딕셔너리
    private readonly Dictionary<IProvideLoad, Action<SaveData>> _dataLoadDictionary = new Dictionary<IProvideLoad, Action<SaveData>>();


    private static SaveData s_saveData;
    public static SaveData sSaveData => s_saveData;

    public override void Init()
    {
        base.Init();
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _isEncrypt, _isBase64);

        SaveData saveData = _fileDataHandler.Load();
        if (saveData != null)
        {
            s_saveData = saveData;
        }
        else
        {
            s_saveData = new SaveData();
        }
    }

    public override void StartManager()
    {

    }

    [ContextMenu("ResetData")]
    public void ResetData()
    {
        _fileDataHandler.DeleteSaveData();
        s_saveData = new SaveData();
    }

    public void SettingDataProvidable(IProvideSave dataProvidable,IProvideLoad dataLoadable)
    {
        Action<SaveData> saveAction = dataProvidable?.GetSaveAction();
        Action<SaveData> loadAction = dataLoadable?.GetLoadAction();

        if(saveAction != null)
        {
            _dataSaveDictionary[dataProvidable] = saveAction;
        }

        if (loadAction != null)
        {
            _dataLoadDictionary[dataLoadable] = loadAction;
        }
    }

    public void SaveData()
    {
        foreach (var dataProvidable in _dataSaveDictionary.Keys)
        {
            _dataSaveDictionary[dataProvidable]?.Invoke(s_saveData);
        }
        _fileDataHandler.Save(s_saveData);
    }

    public void SaveData(IProvideSave providable)
    {
        _dataSaveDictionary[providable]?.Invoke(s_saveData);
        _fileDataHandler.Save(s_saveData);
    }

    public void LoadData()
    {
        s_saveData = _fileDataHandler.Load();

        foreach (var provideSave in _dataSaveDictionary.Keys.ToList())
        {
            var dataProvidable = (IProvideLoad)provideSave;
            _dataLoadDictionary[dataProvidable]?.Invoke(s_saveData);
        }
    }

    public void LoadData(IProvideLoad providable)
    {
        s_saveData = _fileDataHandler.Load();

        _dataLoadDictionary[providable]?.Invoke(s_saveData);
    }

}