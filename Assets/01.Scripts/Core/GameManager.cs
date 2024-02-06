using ManagingSystem;
using Singleton;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public delegate void UnityEventListener();
    
    public event UnityEventListener OnStartEvent = null;

    public PlayerController PlayerController
    {
        get
        {
            if (_playerController == null)
            {
                _playerController = FindObjectOfType<PlayerController>();
            }

            return _playerController;
        }
    }

    private PlayerController _playerController;

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
}