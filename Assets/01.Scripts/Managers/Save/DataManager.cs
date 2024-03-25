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
    }
    public SerializableDictionary<ChapterType, bool> ChapterProgressDictionary;
    public bool IsShowSequence = false;
}


public interface IDataProvidable
{
    public void LoadToDataManager();
    public Action<SaveData> GetProvideAction();
    public Action<SaveData> GetSaveAction();
}


public class DataManager : BaseManager<DataManager>
{
    [Header("SaveSettings")]
    [SerializeField] private string _fileName;
    [SerializeField] private bool _isEncrypt;
    [SerializeField] private bool _isBase64;

    private FileDataHandler _fileDataHandler;

    //얘는 저장하는 이벤트가 달린 딕셔너리
    private Dictionary<IDataProvidable, Action<SaveData>> _dataProvidableDictionary = new Dictionary<IDataProvidable, Action<SaveData>>();
    //얘는 저장된 데이터를 바탕으로 게임에 설정하는 딕셔너리
    private Dictionary<IDataProvidable, Action<SaveData>> _dataSettableDictionary = new Dictionary<IDataProvidable, Action<SaveData>>();


    private static SaveData s_saveData;

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

    public void SettingDataProvidable(IDataProvidable dataProvidable)
    {
        Action<SaveData> saveAction = dataProvidable.GetProvideAction();
        Action<SaveData> loadAction = dataProvidable.GetSaveAction();

        if (_dataProvidableDictionary.ContainsKey(dataProvidable) == false)
            _dataProvidableDictionary.Add(dataProvidable, saveAction);
        else
            _dataProvidableDictionary[dataProvidable] = saveAction;


        if (_dataSettableDictionary.ContainsKey(dataProvidable) == false)
            _dataSettableDictionary.Add(dataProvidable, loadAction);
        else
            _dataSettableDictionary[dataProvidable] = loadAction;
    }

    [ContextMenu("Save")]
    public void SaveData()
    {
        foreach (IDataProvidable dataProvidable in _dataProvidableDictionary.Keys)
        {
            _dataProvidableDictionary[dataProvidable]?.Invoke(s_saveData);
        }
        _fileDataHandler.Save(s_saveData);
    }

    public void SaveData(IDataProvidable providable = null)
    {
        _dataProvidableDictionary[providable]?.Invoke(s_saveData);
        _fileDataHandler.Save(s_saveData);
    }

    [ContextMenu("Load")]

    public void LoadData()
    {
        s_saveData = _fileDataHandler.Load();

        foreach (IDataProvidable dataProvidable in _dataProvidableDictionary.Keys.ToList())
        {
            _dataSettableDictionary[dataProvidable]?.Invoke(s_saveData);
        }
    }

    public void LoadData(IDataProvidable providable = null)
    {
        s_saveData = _fileDataHandler.Load();

        _dataSettableDictionary[providable]?.Invoke(s_saveData);
    }

}