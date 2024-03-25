using System;
using System.Collections.Generic;
using System.Linq;
using ManagingSystem;
using Singleton;
using UnityEngine;

public class SaveData
{
    public SerializableDictionary<ChapterType, bool> ChapterProgressDictionary = new SerializableDictionary<ChapterType, bool>();
}

public interface IDataProvidable
{
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
    private Dictionary<IDataProvidable, Action<SaveData>> _dataSettableDictionary   = new Dictionary<IDataProvidable, Action<SaveData>>();
    private static SaveData s_saveData;

    public override void Init()
    {
        base.Init();
        s_saveData = new SaveData();
        _fileDataHandler = new FileDataHandler(Application.dataPath, _fileName,_isEncrypt,_isBase64);
        SettingDataProvidable();
    }

    public override void StartManager()
    {
    }

    private void SettingDataProvidable()
    {
        var dataProvidableList = FindDataProvdiable();

        foreach(IDataProvidable dataProvidable in dataProvidableList)
        {
            Action<SaveData> saveAction = dataProvidable.GetProvideAction();
            Action<SaveData> loadAction = dataProvidable.GetSaveAction();
            if (_dataProvidableDictionary.ContainsKey(dataProvidable) == false)
            {
                _dataProvidableDictionary.Add(dataProvidable, saveAction);
            }
            if(_dataSettableDictionary.ContainsKey(dataProvidable) == false)
            {
                _dataSettableDictionary.Add(dataProvidable,loadAction);
            }
        }
    }

    public void SaveData(IDataProvidable providable = null)
    {
        List<IDataProvidable> dataProvidableList = new List<IDataProvidable>();

        if(providable != null)
            dataProvidableList.Add(providable);
        else
            dataProvidableList = _dataProvidableDictionary.Keys.ToList();

        foreach(IDataProvidable dataProvidable in dataProvidableList)
        {
            _dataProvidableDictionary[dataProvidable]?.Invoke(s_saveData);
        }
        _fileDataHandler.Save(s_saveData);
    }

    public void LoadData(IDataProvidable providable = null)
    {
        List<IDataProvidable> dataProvidableList = new List<IDataProvidable>();

        if (providable != null)
            dataProvidableList.Add(providable);
        else
            dataProvidableList = _dataProvidableDictionary.Keys.ToList();

        foreach (IDataProvidable dataProvidable in dataProvidableList)
        {
            _dataSettableDictionary[dataProvidable]?.Invoke(s_saveData);
        }
    }

    private List<IDataProvidable> FindDataProvdiable() => FindObjectsOfType<MonoBehaviour>(true).OfType<IDataProvidable>().ToList();
}
