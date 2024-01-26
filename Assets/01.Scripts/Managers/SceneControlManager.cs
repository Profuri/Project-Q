using System;
using ManagingSystem;

public class SceneControlManager : BaseManager<SceneControlManager>
{
    private Scene _currentScene;
    
    public override void StartManager()
    {
        LoadScene(SceneType.Chapter, () =>
        {
            PoolManager.Instance.Pop("Player");
        });
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
}