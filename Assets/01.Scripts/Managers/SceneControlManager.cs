using System;
using ManagingSystem;
using UnityEngine;

public class SceneControlManager : BaseManager<SceneControlManager>
{
    [SerializeField] private SceneType _startSceneType;
    
    private Scene _currentScene;
    public PlayerUnit Player => _currentScene == null ? null : _currentScene.Player;


    [SerializeField] private float _fadeTime;
    [SerializeField] private float _loadingTime;

    private SceneTransitionCanvas _currentCanvas;
    public override void StartManager()
    {
        LoadScene(_startSceneType);
    }

    public void LoadScene(SceneType type, Action onSceneCreate = null, Action onLoadedCallback = null, bool loading = true)
    {
        if (_currentCanvas != null) return;

        _currentCanvas = PoolManager.Instance.Pop("SceneTransitionCanvas") as SceneTransitionCanvas;
        _currentCanvas.PresentTransition(0.0f, 1.0f, _fadeTime, () =>
        {
            if (_currentScene is not null)
            {
                PoolManager.Instance.Push(_currentScene);
            }

            _currentScene = PoolManager.Instance.Pop($"{type}Scene") as Scene;
            onSceneCreate?.Invoke();

            //위에 함수가 전부다 정상 작동 했을 경우 밑에 있는 것을 실행시켜주어야 함
            _currentCanvas.PauseTransition(_loadingTime, () =>
            {
                _currentCanvas.PresentTransition(1.0f, 0.0f, _fadeTime, () =>
                {
                    onLoadedCallback?.Invoke();
                    PoolManager.Instance.Push(_currentCanvas);
                    _currentCanvas = null;
                });
            });
        });
    }

    public PoolableMono AddObject(string id)
    {
        if (_currentScene is null)
        {
            Debug.LogError("[SceneControlManager] currentScene doesn't loaded. returning null");
            return null;
        }
        return _currentScene.AddObject(id);
    }

    public void DeleteObject(PoolableMono obj)
    {
        if (_currentScene is null)
        {
            Debug.LogError("[SceneControlManager] currentScene doesnt loaded.");
            return;
        }
        _currentScene.DeleteObject(obj);
    }
}