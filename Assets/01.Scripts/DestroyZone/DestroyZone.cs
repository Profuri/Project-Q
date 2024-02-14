using AxisConvertSystem;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ObjectUnit>(out var unit))
        {
            unit.ReloadUnit();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var col = GetComponent<Collider>();

        if (col is null)
        {
            return;
        }
        
        Gizmos.color = new Color(1f, 0.41f, 0.21f);
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
#endif
}
