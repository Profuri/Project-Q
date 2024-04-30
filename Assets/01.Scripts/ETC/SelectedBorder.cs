using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectedBorder : PoolableMono
{
    [SerializeField] private float _activeTime = 0.25f;

    private Material _material;
    private readonly int _activeProgressHash = Shader.PropertyToID("_ActiveProgress");
    private readonly int _alphaProgressHash = Shader.PropertyToID("_Alpha");

    private Collider _col;
    private bool _active = false;
    public float Distance
    {
        get
        {
            var playerPos = SceneControlManager.Instance.CurrentScene.Player.Collider.bounds.center;
            playerPos.y = 0;
            var curPos = _col.bounds.center;
            curPos.y = 0;

            return Vector3.Distance(playerPos, curPos);
        }
    }

    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        _material = renderer.material;
    }

    public override void OnPop()
    {
        _active = false;
        Activate(_active);
    }

    public override void OnPush()
    {
    }

    public void Setting(Collider col)
    {
        _col = col;
        var bounds = col.bounds;
        var position = bounds.center;
        var size = bounds.size + new Vector3(0.1f, 0.1f, 0.1f);

        transform.position = position;
        transform.localScale = size;
        transform.SetParent(col.transform);
    }

    public void Activate(bool active)
    {
        if (_active == active)
        {
            return;
        }
        
        _active = active;
        CoroutineManager.Instance.StartSafeCoroutine(GetInstanceID(), ActiveRoutine(active));
    }

    public void SetAlpha(float alpha)
    {
        _material.SetFloat(_alphaProgressHash, alpha);
    }
    
    private IEnumerator ActiveRoutine(bool active)
    {
        var initProgress = _material.GetFloat(_activeProgressHash);
        var targetProgress = active ? 1f : 0f;

        var time = _activeTime * Mathf.Abs(targetProgress - initProgress);
        var currentTime = 0f;
            
        while (currentTime <= time)
        {
            currentTime += Time.deltaTime;
            var percent = currentTime / time;
            _material.SetFloat(_activeProgressHash, Mathf.Lerp(initProgress, targetProgress, percent));
            yield return null;
        }
    }
}
