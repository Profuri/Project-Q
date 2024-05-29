using AxisConvertSystem;
using UnityEngine;

public class UnClimbableEffect : PoolableMono
{
    [SerializeField] private float _findPlayerRadius = 1.5f;
    private readonly int _alphaHash = Shader.PropertyToID("_Alpha"); 
    private Renderer _renderer;

    public override void OnPop()
    {
        _renderer = GetComponent<Renderer>();
        SetAlpha(0f);
    }

    public override void OnPush()
    {

    }

    private void Update()
    {
        PlayerUnit playerUnit = SceneControlManager.Instance.Player;

        if(playerUnit != null && playerUnit.IsControllingAxis)
        {
            SetAlpha(1f);
            return;
        }
        
        float percent = CalculateDistancePercent(playerUnit.Collider.bounds.center);
        SetAlpha(percent);
    }
    
    public void SetAlpha(float alpha)
    {
        _renderer.material.SetFloat(_alphaHash, alpha * 0.5f);
    }
    
    private float CalculateDistancePercent(Vector3 targetPos)
    {
        float distance = Vector3.Distance(transform.position,targetPos);
        return 1 - distance / _findPlayerRadius;
    }

    public void Setting(ObjectUnit owner)
    {
        Collider col = owner.Collider;
        const float yOffset = 0.1f;

        Vector3 size = col.bounds.size;
        size.y = 0.1f;
        Vector3 position = col.bounds.center;
        position.y = col.bounds.max.y + yOffset;

        transform.localScale = new Vector3(size.x, size.z, size.y);
        transform.position = position;
        transform.SetParent(col.transform);
    }
}