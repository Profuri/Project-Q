using System;
using System.Collections;
using UnityEngine;

public class BridgeObject : PoolableMono
{
    public void SetWidth(float width)
    {
        var scale = transform.localScale;
        scale.z = width;
        transform.localScale = scale;
    }
    
    public void Generate(Vector3 position, Quaternion rotation)
    {
        transform.position = position - Vector3.up * 5;
        transform.rotation = rotation;
        StartCoroutine(MoveRoutine(position, null));
    }

    public void Remove()
    {
        StartCoroutine(MoveRoutine(transform.position - Vector3.up * 5, () =>
        {
            SceneControlManager.Instance.DeleteObject(this);
        }));
    }

    private IEnumerator MoveRoutine(Vector3 dest, Action CallBack)
    {
        while (true)
        {
            var pos = transform.position;
            var lerp = Vector3.Lerp(pos, dest, 0.1f);
            transform.position = lerp;
            
            if (Vector3.Distance(pos, dest) <= 0.01f)
            {
                break;
            }

            yield return null;
        }
        
        transform.position = dest;
        CallBack?.Invoke();
    }

    public override void OnPop()
    {
    }

    public override void OnPush()
    {
    }
}
