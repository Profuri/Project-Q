using AxisConvertSystem;
using UnityEngine;

public class UnClimbableEffect : PoolableMono
{
    [SerializeField] private float _findPlayerRadius = 1.5f;
    private readonly int _alphaHash = Shader.PropertyToID("_Alpha"); 
    private Renderer _renderer;
    private ObjectUnit _owner;

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
        PlayerUnit unit = FindNearUnits();
        if(unit != null)
        {
            Collider col = unit.Collider;
            float percent = CalculateDistancePercent(col.bounds.center);

            SetAlpha(percent);
        }
    }
    public void SetAlpha(float alpha)
    {
        Material mat = _renderer.material;
        mat.SetFloat(_alphaHash,alpha * 0.5f);
        _renderer.material = mat;
    }
    private float CalculateDistancePercent(Vector3 targetPos)
    {
        float distance = Vector3.Distance(transform.position,targetPos);

        return 1 - distance / _findPlayerRadius;
    }
    private PlayerUnit FindNearUnits()
    {
        Vector3 origin = transform.position;
        float radius = _findPlayerRadius;
        int layer = 1 << LayerMask.NameToLayer("Player");
        Collider[] cols = Physics.OverlapSphere(origin,radius,layer);

        foreach(Collider col in cols)
        {
            if(col.TryGetComponent(out PlayerUnit playerUnit)) 
            {
                return playerUnit;
            }
        } 
        return null;
    }
    public void Setting(ObjectUnit owner)
    {
        this._owner = owner;
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