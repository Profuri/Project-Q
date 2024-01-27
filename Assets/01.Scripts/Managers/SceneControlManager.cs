using System;
using ManagingSystem;
using UnityEngine;

public class SceneControlManager : BaseManager<SceneControlManager>
{
    private Scene _currentScene;
    
    public override void StartManager()
    {
        LoadScene(SceneType.Chapter);
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