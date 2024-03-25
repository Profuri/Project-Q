using System;
using ManagingSystem;
using UnityEngine;

public class SceneControlManager : BaseManager<SceneControlManager>
{
    private Scene _currentScene;
    public PlayerUnit Player => _currentScene == null ? null : _currentScene.Player;


    private SceneTransitionCanvas _currentCanvas;
    public override void StartManager()
    {
        LoadScene(SceneType.Chapter);
    }

    public void LoadScene(SceneType type, Action onLoadedCallback = null, float loadingTime = 1.5f)
    {
        if (_currentCanvas != null) return;


        _currentCanvas = PoolManager.Instance.Pop("SceneTransitionCanvas") as SceneTransitionCanvas;
        _currentCanvas.PresentTransition(SceneTransitionCanvas.sMaxSize, Vector2.zero, 1.5f, () =>
        {
            if (_currentScene is not null)
            {
                PoolManager.Instance.Push(_currentScene);
            }

            _currentScene = PoolManager.Instance.Pop($"{type}Scene") as Scene;
            onLoadedCallback?.Invoke();

            //위에 함수가 전부다 정상 작동 했을 경우 밑에 있는 것을 실행시켜주어야 함
            _currentCanvas.PauseTransition(loadingTime, () =>
            {
                _currentCanvas.PresentTransition(Vector2.zero, SceneTransitionCanvas.sMaxSize, 1.5f, () =>
                {
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