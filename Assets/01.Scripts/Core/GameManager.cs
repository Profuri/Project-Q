using System;
using AxisConvertSystem;
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

    private bool _inPause;
    public bool InPause
    {
        get => _inPause;
        set
        {
            _inPause = value;
            Time.timeScale = _inPause ? 0 : 1;
            InputManager.Instance.SetEnableInputAll(!_inPause);
        }
    }

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
        if (InPause || StageManager.Instance.CurrentStageAxis != AxisType.None)
        {
            return;
        }

        InPause = true;        
        UIManager.Instance.GenerateUI("PauseWindow");
    }

    public void Resume()
    {
        if(!InPause)
        {
            return;
        }
        
        InPause = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}