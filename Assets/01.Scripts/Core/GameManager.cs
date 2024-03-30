using AxisConvertSystem;
using DG.Tweening;
using ManagingSystem;
using Singleton;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private CoreData _coreData;
    public CoreData CoreData => _coreData;
    
    public delegate void UnityEventListener();
    public event UnityEventListener OnStartEvent = null;

    public bool InPause { get; set; }
    
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
        if (InPause || TutorialManager.Instance.OnTutorial || StageManager.Instance.CurrentStageAxis != AxisType.None)
        {
            return;
        }

        InPause = true;       
        Time.timeScale = 0;
        InputManager.Instance.SetEnableInputAll(false);
        UIManager.Instance.GenerateUI("PauseWindow");

        CursorManager.SetCursorEnable(true);
        CursorManager.SetCursorLockState(CursorLockMode.None);
    }

    public void Resume(bool stateSettingSelf = true)
    {
        if(!InPause)
        {
            return;
        }

        if (stateSettingSelf)
        {
            InPause = false;
        }
        
        Time.timeScale = 1;
        InputManager.Instance.SetEnableInputAll(true);


        CursorManager.SetCursorEnable(false);
        CursorManager.SetCursorLockState(CursorLockMode.Locked);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}