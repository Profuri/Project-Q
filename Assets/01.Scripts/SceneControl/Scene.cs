using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scene : PoolableMono
{
    [SerializeField] private SceneType _type;
    public SceneType Type => _type;
    
    public PlayerController Player { get; private set; }

    private List<PoolableMono> _objects;

    private void Awake()
    {
        _objects = new List<PoolableMono>();
    }

    public override void OnPop()
    {
        Player = AddObject("Player") as PlayerController;
    }

    public override void OnPush()
    {
        Player.InputReader.ClearInputEvent();
        while (_objects.Count > 0)
        {
            DeleteObject(_objects.First());
        }
        _objects.Clear();
    }

    public PoolableMono AddObject(string id)
    {
        var obj = PoolManager.Instance.Pop(id);
        _objects.Add(obj);
        return obj;
    }

    public void DeleteObject(PoolableMono obj)
    {
        PoolManager.Instance.Push(obj);
        _objects.Remove(obj);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        gameObject.name = $"{_type}Scene";
    }

#endif
}