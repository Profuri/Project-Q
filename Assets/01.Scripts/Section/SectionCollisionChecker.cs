using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SectionCollisionChecker : MonoBehaviour
{
    [SerializeField] private Vector3 _boundSize;
    
    private Section _section;
    private BoxCollider _boundCollider;

    private void Awake()
    {
        _section = transform.GetComponent<Section>();
        _boundCollider = GetComponent<BoxCollider>();
        _boundCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_section.Active && other.TryGetComponent<PlayerUnit>(out var player))
        {
            _section.OnEnter(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_section.Active && !_section.Lock && other.TryGetComponent<PlayerUnit>(out var player))
        {
            _section.OnExit(player);
        }
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        GetComponent<BoxCollider>().size = _boundSize;
    }

#endif
}
