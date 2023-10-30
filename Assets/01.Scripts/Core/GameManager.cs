using System;
using Singleton;

public class GameManager : MonoSingleton<GameManager>
{
    public delegate void UnityEventListener();
    
    public event UnityEventListener OnStartEvent = null;
    public event UnityEventListener OnUpdateEvent = null;

    private void Start()
    {
        OnStartEvent?.Invoke();
    }

    private void Update()
    {
        OnUpdateEvent?.Invoke();
    }
}