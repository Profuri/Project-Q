using System.Collections.Generic;
using System.Linq;
using AxisConvertSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Scene : PoolableMono
{
    [SerializeField] private SceneType _type;
    
    public PlayerUnit Player { get; private set; }
    private List<PoolableMono> _objects;

    public Section initSection;

    public UnityEvent onLoadScene = null;
    public UnityEvent onDestroyScene = null;

    private UIComponent _keyInfoPanel;

    protected virtual void Awake()
    {
        _objects = new List<PoolableMono>();
    }

    public override void OnPop()
    {
        if (_type != SceneType.Title)
        {
            if (_type == SceneType.Stage)
            {
                InputManager.Instance.CameraInputReader.OnZoomOutEvent += CameraManager.Instance.ZoomOutCamera;
                InputManager.Instance.CameraInputReader.OnZoomInEvent += CameraManager.Instance.ZoomInCamera;
            }
            InputManager.Instance.UIInputReader.OnPauseClickEvent += GameManager.Instance.Pause;    
        }
    }

    public override void OnPush()
    {
        _keyInfoPanel.Disappear();
        PoolManager.Instance.Push(Player);
        Player = null;
        
        InputManager.Instance.CameraInputReader.OnZoomOutEvent -= CameraManager.Instance.ZoomOutCamera;
        InputManager.Instance.CameraInputReader.OnZoomInEvent -= CameraManager.Instance.ZoomInCamera;
        InputManager.Instance.UIInputReader.OnPauseClickEvent -= GameManager.Instance.Pause;
        
        while (_objects.Count > 0)
        {
            var firstObj = _objects.First();
            if (firstObj is Stage stage)
            {
                SafeDeleteObject(stage);
            }
            else
            {
                DeleteObject(firstObj);
            }
        }
        _objects.Clear();
        
        onDestroyScene?.Invoke();
    }

    //플레이어 땅으로 떨구는 함수
    public void CreatePlayer()
    {
        if (Player is not null)
        {
            Debug.LogError("[Scene] Already create player.");
            return;
        }
        
        InputManager.Instance.SetEnableInputAll(false);
        Player = AddObject("Player") as PlayerUnit;
        Player.transform.localPosition = initSection.PlayerResetPoint;
        
        Player.ModelTrm.localPosition += Vector3.up * 5;
        Player.ModelTrm.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            var landParticle = SceneControlManager.Instance.AddObject("PlayerLandParticle") as PoolableParticle;
            landParticle.SetPositionAndRotation(Player.transform.position, Quaternion.identity);
            landParticle.Play();
            SoundManager.Instance.PlaySFX("PlayerAfterJump");
            _keyInfoPanel = UIManager.Instance.GenerateUI($"{_type}KeyInfoPanel");
            _keyInfoPanel.ResetPosition();
            if (!StoryManager.Instance.IsPlay)
            {
                InputManager.Instance.SetEnableInputAll(true);
            }
        });
    }

    public PoolableMono AddObject(string id)
    {
        var obj = PoolManager.Instance.Pop(id);
        _objects.Add(obj);
        return obj;
    }

    public void DeleteObject(PoolableMono obj)
    {
        _objects.Remove(obj);
        
        if (!obj.poolOut)
        {
            return;
        }
        
        PoolManager.Instance.Push(obj);
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