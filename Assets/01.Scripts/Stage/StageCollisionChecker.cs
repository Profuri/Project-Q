using System;
using UnityEngine;

public class StageCollisionChecker : MonoBehaviour
{
    private Stage _stage;

    private void Awake()
    {
        _stage = transform.GetComponent<Stage>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent<PlayerController>(out var player))
        {
            return;
        }
        
        player.PlayerUnit.SettingStage();
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
}
