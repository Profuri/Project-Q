using System;
using System.Collections;
using System.Collections.Generic;
using StageStructureConvertSystem;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<StructureObjectUnitBase>(out var unit))
        {
            unit.ReloadObject();
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