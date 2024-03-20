using System;
using System.Collections.Generic;
using System.Linq;
using ManagingSystem;
using UnityEngine;

public class PoolManager : BaseManager<PoolManager>
{
    private readonly Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    [SerializeField] private List<PoolingList> _poolingLists;
    
    public override void StartManager()
    {
        foreach (var pair in _poolingLists.SelectMany(poolingList => poolingList.poolingItems))
        {
            CreatePool(pair.prefab, transform, pair.cnt);
        }
    }
        
    private void CreatePool(PoolableMono prefab, Transform parent, int cnt)
    {
        if (_pools.ContainsKey(prefab.name))
        {
            return;
        }
        
        _pools.Add(prefab.name, new Pool(prefab, parent, cnt));
    }

    public PoolableMono Pop(string key)
    {
        if (_pools.TryGetValue(key, out var pool))
        {
            return pool.Pop();
        }
        
        Debug.LogError($"[PoolManager] Doesn't exist key on pools : [{key}]");
        return null;
    }

    public void Push(PoolableMono obj)
    {
        _pools[obj.name].Push(obj);
    }

    public void SettingPoolinglist(List<PoolingList> poolingListList)
    {
        _poolingLists = poolingListList;
    }
}