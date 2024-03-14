using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnClimbableEffect : PoolableMono
{
    public override void OnPop()
    {
        
    }

    public override void OnPush()
    {
        
    }

    public void Setting(Collider collider)
    {
        float xMax = collider.bounds.max.x;
        float xMin = collider.bounds.min.x;
        float yMax = collider.bounds.max.z;
        float yMin = collider.bounds.min.z;
        const float yOffset = 0.1f;

        Vector3 position = collider.bounds.center;
        position.y = collider.bounds.max.y + yOffset;

        transform.localScale = new Vector3(Mathf.Abs(xMax - xMin),Mathf.Abs(yMin - yMax),0.1f);
        transform.position = position;
        transform.SetParent(collider.transform);
    }
}
