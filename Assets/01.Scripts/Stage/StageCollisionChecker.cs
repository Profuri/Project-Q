using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StageCollisionChecker : MonoBehaviour
{
    [SerializeField] private Vector3 _stageBoundSize;
    
    private Stage _stage;
    private BoxCollider _stageBoundCollider;

    private void Awake()
    {
        _stage = transform.GetComponent<Stage>();
        _stageBoundCollider = GetComponent<BoxCollider>();
        _stageBoundCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_stage.ActiveStage && other.TryGetComponent<PlayerController>(out var player))
        {
            _stage.StageEnterEvent(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_stage.ActiveStage && other.TryGetComponent<PlayerController>(out var player))
        {
            _stage.StageExitEvent(player);
        }
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        GetComponent<BoxCollider>().size = _stageBoundSize;
    }

#endif
}
