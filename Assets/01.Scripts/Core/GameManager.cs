using ManagingSystem;
using Singleton;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public delegate void UnityEventListener();
    
    public event UnityEventListener OnStartEvent = null;
    public event UnityEventListener OnUpdateEvent = null;

    private void Awake()
    {
        var managers = GetComponentsInChildren<IManager>();
        foreach (var manager in managers)
        {
            manager.Init();
        }
    }

    private void Start()
    {
        OnStartEvent?.Invoke();
        PoolManager.Instance.Pop("Player");
    }

    private void Update()
    {
        OnUpdateEvent?.Invoke();
    }
}