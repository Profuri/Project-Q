using AxisConvertSystem;
using UnityEngine;

public class SelectedBorder : PoolableMono
{
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

        float originMagnitude = Vector3.one.magnitude;
        float currentMagnitude = size.magnitude;
        float percent = currentMagnitude / originMagnitude;

        float offset = _initCornerSize * 0.5f;

        if (percent > 1f)
        {
            percent = 1f - (percent - 1f);
        }
        else
        {
            percent = 1f + (1f - percent);
        }

        float cornerSize = _initCornerSize * percent + offset;

        _material.SetFloat(_cornerSizeHash, cornerSize);

        transform.position = position;
        transform.localScale = size;
    }

    public void Activate(bool active)
    {
        // if (_active == active)
        // {
        //     return;
        // }
        _active = active;
    }
    
    public void SetDistanceProgress(float alpha, bool activePower)
    {
        if (activePower)
        {
            const float maxPower = 1f;

            _material.SetFloat(_alphaProgressHash, maxPower);
            _material.SetFloat(_activeProgressHash, maxPower);
            return;
        }
        else
        {
            _material.SetFloat(_activeProgressHash, 0f);
            _material.SetFloat(_alphaProgressHash, 0f);
        }
        _material.SetFloat(_alphaProgressHash, alpha);

    }
}
