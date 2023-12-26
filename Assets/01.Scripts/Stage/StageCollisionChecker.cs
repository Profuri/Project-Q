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
        if (_stage.ActiveStage)
        {
            return;
        }
        
        if(!other.TryGetComponent<PlayerController>(out var player))
        {
            return;
        }
        
        StageManager.Instance.ChangeToNextStage(player);
        // camera setting
        // stage init setting
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out var player))
        {
            return;
        }
        
        // camera setting
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        GetComponent<BoxCollider>().size = _stageBoundSize;
    }

#endif
}
