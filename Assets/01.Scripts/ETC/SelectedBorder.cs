using AxisConvertSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectedBorder : PoolableMono
{
    [SerializeField] private float _activeTime = 0.25f;
    //[SerializeField] private float _cornerSizeOffset = 0.5f;

    private Material _material;

    private readonly int _cornerSizeHash = Shader.PropertyToID("_CornerSize");
    private readonly int _activeProgressHash = Shader.PropertyToID("_ActiveProgress");
    private readonly int _alphaProgressHash = Shader.PropertyToID("_Alpha");

    private ObjectUnit _owner;
    private bool _active = false;

    private float _initCornerSize;

    public float Distance
    {
        get
        {
            var playerPos = SceneControlManager.Instance.CurrentScene.Player.Collider.bounds.center;
            playerPos.y = 0;
            var curPos = _owner.Collider.bounds.center;
            curPos.y = 0;

            return Vector3.Distance(playerPos, curPos);
        }
    }

    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        _material = renderer.material;
        _initCornerSize = _material.GetFloat(_cornerSizeHash);
    }

    private void Update()
    {
        if(_owner)
        {
            transform.position = _owner.Collider.bounds.center;
        }
    }

    public override void OnPop()
    {
        _active = false;
        Activate(_active);
    }

    public override void OnPush()
    {
        _owner = null;
    }

    public void Setting(ObjectUnit owner)
    {
        _owner = owner;
        var bounds = owner.Collider.bounds;
        var position = bounds.center;
        var size = owner.BeforeConvertedUnitInfo.ColliderBoundSize + new Vector3(0.1f, 0.1f, 0.1f);

        //float cornerSize = _initCornerSize / (size.y >= 1 ? size.y + _cornerSizeOffset : size.y - _cornerSizeOffset);
        //_material.SetFloat(_cornerSizeHash, cornerSize);

        transform.position = position;
        transform.localScale = size;
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
