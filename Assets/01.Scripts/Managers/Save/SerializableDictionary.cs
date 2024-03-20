using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> _keyList = new List<TKey>();
    [SerializeField] private List<TValue> _valueList = new List<TValue>();


    /// <summary>
    /// 저장하기 전에 해 줘야 하는 일
    /// </summary>

    public void OnBeforeSerialize()
    {
        _keyList.Clear();
        _valueList.Clear();

        foreach (var kvp in this)
        {
            _keyList.Add(kvp.Key);
            _valueList.Add(kvp.Value);
        }
    }

    /// <summary>
    /// 불러온 뒤에 해줄 일
    /// </summary>
    public void OnAfterDeserialize()
    {
        this.Clear();
        if (_keyList.Count != _valueList.Count)
        {
            Debug.LogError("Key Count does not match to value count");
        }
        else
        {
            for (int i = 0; i < _keyList.Count; i++)
            {
                this.Add(_keyList[i], _valueList[i]);
            }
        }
    }



}
