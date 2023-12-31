using ManagingSystem;
using Singleton;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public delegate void UnityEventListener();
    public delegate void ObserverListener<in T>(T value);
    
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
    }

    private void Update()
    {
        OnUpdateEvent?.Invoke();
    }
}