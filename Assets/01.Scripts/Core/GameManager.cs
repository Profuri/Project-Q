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
        CursorManager.ReloadCursor();
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
        InputManager.Instance.SetEnableInputWithout(EInputCategory.Escape,false);
        UIManager.Instance.GenerateUI("PauseWindow");
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
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");
        Application.Quit();
    }
}