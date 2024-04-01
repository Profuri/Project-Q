using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Scene : PoolableMono
{
    [SerializeField] private SceneType _type;
    
    public PlayerUnit Player { get; private set; }
    private List<PoolableMono> _objects;

    public UnityEvent onLoadScene = null;
    public UnityEvent onDestroyScene = null;

    private void Awake()
    {
        _objects = new List<PoolableMono>();
    }

    public override void OnPop()
    {
        Player = AddObject("Player") as PlayerUnit;
        Player.transform.localPosition = Vector3.zero;

        if (_type != SceneType.Title)
        {
            InputManager.Instance.InputReader.OnPauseClickEvent += GameManager.Instance.Pause;    
        }
    }

    public override void OnPush()
    {
        PoolManager.Instance.Push(Player);
        
        InputManager.Instance.InputReader.ClearPlayerInputEvent();
        InputManager.Instance.InputReader.OnPauseClickEvent -= GameManager.Instance.Pause;
        
        while (_objects.Count > 0)
        {
            DeleteObject(_objects.First());
        }
        _objects.Clear();
        
        onDestroyScene?.Invoke();
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

    public void SafeDeleteObject(PoolableMono obj)
    {
        Destroy(obj.gameObject);
        _objects.Remove(obj);
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        gameObject.name = $"{_type}Scene";
    }

#endif
}