using System;
using ManagingSystem;
using UnityEngine;

public class SceneControlManager : BaseManager<SceneControlManager>
{
    [SerializeField] private SceneType _startSceneType;
    
    private Scene _currentScene;
    public PlayerUnit Player => _currentScene == null ? null : _currentScene.Player;
    
    public override void StartManager()
    {
        LoadScene(_startSceneType);
    }

    public void LoadScene(SceneType type, Action onLoadedCallback = null, bool loading = true)
    {
        if (_currentScene is not null)
        {
            PoolManager.Instance.Push(_currentScene);
        }

        _currentScene = PoolManager.Instance.Pop($"{type}Scene") as Scene;
        onLoadedCallback?.Invoke();
    }

    public PoolableMono AddObject(string id)
    {
        if (_currentScene is null)
        {
            Debug.LogError("[SceneControlManager] currentScene doesnt loaded. returning null");
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