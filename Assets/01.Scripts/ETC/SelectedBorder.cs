using AxisConvertSystem;
using UnityEngine;

public class SelectedBorder : PoolableMono
{
    private Material _material;

    private readonly int _cornerSizeHash = Shader.PropertyToID("_CornerSize");
    private readonly int _activeProgressHash = Shader.PropertyToID("_ActiveProgress");
    private readonly int _alphaProgressHash = Shader.PropertyToID("_Alpha");
    
    private ObjectUnit _owner;

    private const float InitCornerSize = 0.075f;

    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        _material = renderer.material;
    }

    private void Update()
    {
        if(_owner)
        {
            _owner.GetCenterPosAndSize(out var center, out var size);
            transform.position = center;
        }
    }

    public override void OnPop()
    {
    }

    public override void OnPush()
    {
        _owner = null;
    }

    public void Setting(ObjectUnit owner)
    {
        _owner = owner;
        
        owner.GetCenterPosAndSize(out var position, out var size);
        size += new Vector3(0.1f, 0.1f, 0.1f);

        float originMagnitude = Vector3.one.magnitude;
        float currentMagnitude = size.magnitude;
        float percent = currentMagnitude / originMagnitude;

        float offset = InitCornerSize * 0.5f;

        if (percent > 1f)
        {
            percent = 1f - (percent - (int)percent);
        }
        else
        {
            percent = 1f + (1f - percent);
        }

        float cornerSize = InitCornerSize * percent + offset;

        _material.SetFloat(_cornerSizeHash, cornerSize);

        transform.position = position;
        transform.localScale = size;
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
        
        _material.SetFloat(_activeProgressHash, 0f);
        _material.SetFloat(_alphaProgressHash, 0f);
        _material.SetFloat(_alphaProgressHash, alpha);
    }
}
