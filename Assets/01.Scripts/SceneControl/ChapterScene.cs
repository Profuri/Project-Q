using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Diagnostics;

public class ChapterScene : Scene, IDataProvidable
{
    private Dictionary<ChapterType, Chapter> _chapterDictionary;
    private Dictionary<ChapterType, bool> _chapterSequenceDictionary;

    public UnityEvent<ChapterType> OnChapterClear;
    public UnityEvent OnSubChaptersClear;
    
    public override void OnPop()
    {
        base.OnPop();
        _chapterDictionary = new Dictionary<ChapterType, Chapter>();

        var chapters = GetComponentsInChildren<Chapter>();

        foreach( var chapter in chapters )
        {
            ChapterType chapterType = chapter.Data.chapter;
            _chapterDictionary.Add(chapterType,chapter);
        }
        LoadToDataManager();

        DataManager.Instance.LoadData(this);
    }

    private void LoadChapterEvents(SaveData saveData)
    {
        Dictionary<ChapterType,bool> dictionary = saveData.ChapterProgressDictionary;
        _chapterSequenceDictionary = saveData.ChapterSequenceDictionary as Dictionary<ChapterType,bool>;

        int clearCnt = 0;
        foreach(KeyValuePair<ChapterType,bool> kvp in dictionary)
        {
            ChapterType chapterType = kvp.Key;

            bool isShowingSequence = false;
            if(_chapterSequenceDictionary.ContainsKey(chapterType))
            {
                isShowingSequence = _chapterSequenceDictionary[chapterType];
            }
            else
            {
                _chapterSequenceDictionary.Add(chapterType, false);
            }

            if (kvp.Value && !isShowingSequence)
            {
                clearCnt++;
                OnChapterClear?.Invoke(chapterType);
                _chapterSequenceDictionary[chapterType] = true;
            }
        }
        if(clearCnt > 4)
        {
            OnSubChaptersClear?.Invoke();
        }
    }

    public Action<SaveData> GetProvideAction()
    {
        return (saveData) =>
        {
            var dictionary = saveData.ChapterSequenceDictionary;
            foreach(var kvp in _chapterSequenceDictionary)
            {
                if(dictionary.ContainsKey(kvp.Key))
                {
                    dictionary[kvp.Key] = kvp.Value;
                }
                else
                {
                    dictionary.Add(kvp.Key, kvp.Value);
                }
            }
        };
    }

    public Action<SaveData> GetSaveAction()
    {
        return (saveData) =>
        {
            LoadChapterEvents(saveData);
        };
    }
    
    public void LoadToDataManager()
    {
        DataManager.Instance.SettingDataProvidable(this);
    }
}
