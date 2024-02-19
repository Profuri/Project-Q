using ManagingSystem;
using Singleton;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private CoreData _coreData;
    public CoreData CoreData => _coreData;
    
    public delegate void UnityEventListener();
    public event UnityEventListener OnStartEvent = null;

    public PlayerUnit PlayerUnit
    {
        get
        {
            if (_playerUnit == null)
            {
                _playerUnit = FindObjectOfType<PlayerUnit>();
            }

            return _playerUnit;
        }
    }

    private PlayerUnit _playerUnit;

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