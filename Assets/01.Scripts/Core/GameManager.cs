using System;
using DG.Tweening;
using ManagingSystem;
using Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private CoreData _coreData;
    public CoreData CoreData => _coreData;
    
    public delegate void UnityEventListener();
    public event UnityEventListener OnStartEvent = null;
    
    public bool InPause { get; private set; }

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
        DOTween.Init(true, true, LogBehaviour.Verbose). SetCapacity(2000, 100);
        
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

    public void Pause()
    {
        if (InPause)
        {
            return;
        }
        
        InPause = true;
        Time.timeScale = 0f;
        InputManager.Instance.SetEnableInputAll(false);
        UIManager.Instance.GenerateUI("PauseWindow");
    }

    public void Resume()
    {
        if(!InPause)
        {
            return;
        }
        
        InPause = false;
        InputManager.Instance.SetEnableInputAll(true);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}