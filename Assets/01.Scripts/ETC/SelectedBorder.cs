using UnityEngine;

public class SelectedBorder : PoolableMono
{
    public override void OnPop()
    {
    }

    public override void OnPush()
    {
    }

    public void Setting(Collider col)
    {
        var bounds = col.bounds;
        var position = bounds.center;
        var size = bounds.size + new Vector3(0.1f, 0.1f, 0.1f);

        transform.position = position;
        transform.localScale = size;
        transform.SetParent(col.transform);
    }
}
