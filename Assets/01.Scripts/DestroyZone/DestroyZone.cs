using AxisConvertSystem;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ObjectUnit>(out var unit))
        {
            var rigid = other.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.velocity = Vector3.zero;
            }
            
            // unit.ReloadObject();
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
