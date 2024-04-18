using UnityEngine;

public class UnClimbableEffect : PoolableMono
{
    public override void OnPop()
    {
        
    }

    public override void OnPush()
    {
        
    }

    public void Setting(Collider col)
    {
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