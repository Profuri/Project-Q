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
        ChapterProgressDictionary = new SerializableDictionary<ChapterType, bool>();
        VolumeDictionary = new SerializableDictionary<EAUDIO_MIXER, float>();

        resolutionIndex = (uint)Screen.resolutions.Length - 1;
    }

    public SerializableDictionary<ChapterType, bool> ChapterProgressDictionary;
    public SerializableDictionary<EAUDIO_MIXER, float> VolumeDictionary;
    public bool IsShowSequence = false;

    public bool DefaultFullScreen;
    public QualityType DefaultQuality;
    public uint resolutionIndex;
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
    private Dictionary<IProvideSave, Action<SaveData>> _dataSaveDictionary = new Dictionary<IProvideSave, Action<SaveData>>();

    //얘는 저장된 데이터를 바탕으로 게임에 설정하는 딕셔너리
    private Dictionary<IProvideLoad, Action<SaveData>> _dataLoadDictionary = new Dictionary<IProvideLoad, Action<SaveData>>();


    private static SaveData s_saveData;
    public static SaveData sSaveData => s_saveData;

    public override void Init()
    {
        base.Init();
        _fileDataHandler = new FileDataHandler(Application.dataPath, _fileName, _isEncrypt, _isBase64);

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

    public void SettingDataProvidable(IProvideSave dataProvidable,IProvideLoad dataLoadable)
    {
        Action<SaveData> saveAction = dataProvidable?.GetSaveAction();
        Action<SaveData> loadAction = dataLoadable?.GetLoadAction();


        if(saveAction != null)
        {
            if (_dataSaveDictionary.ContainsKey(dataProvidable) == false)
            {
                _dataSaveDictionary.Add(dataProvidable, saveAction);
            }
            else
            {
                _dataSaveDictionary[dataProvidable] = saveAction;
                Debug.LogError($"Dictionary has value: {dataProvidable}");
            }
        }

        if (loadAction != null)
        {
            if (_dataLoadDictionary.ContainsKey(dataLoadable) == false)
            {
                _dataLoadDictionary.Add(dataLoadable, loadAction);
            }
            else
            {
                _dataLoadDictionary[dataLoadable] = loadAction;
                Debug.LogError($"Dictionary has value: {dataLoadable}");
            }

        }
    }


    [ContextMenu("Save")]
    public void SaveData()
    {
        foreach (IProvideSave dataProvidable in _dataSaveDictionary.Keys)
        {
            _dataSaveDictionary[dataProvidable]?.Invoke(s_saveData);
        }
        _fileDataHandler.Save(s_saveData);
    }

    public void SaveData(IProvideSave providable = null)
    {
        _dataSaveDictionary[providable]?.Invoke(s_saveData);
        _fileDataHandler.Save(s_saveData);
    }

    [ContextMenu("Load")]

    public void LoadData()
    {
        s_saveData = _fileDataHandler.Load();

        foreach (IProvideLoad dataProvidable in _dataSaveDictionary.Keys.ToList())
        {
            _dataLoadDictionary[dataProvidable]?.Invoke(s_saveData);
        }
    }

    public void LoadData(IProvideLoad providable = null)
    {
        s_saveData = _fileDataHandler.Load();

        _dataLoadDictionary[providable]?.Invoke(s_saveData);
    }

}