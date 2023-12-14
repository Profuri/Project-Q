using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterCheck : MonoBehaviour
{
    Stage _stage;
    private void Awake()
    {
        _stage = transform.parent.GetComponent<Stage>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player;
        if(other.TryGetComponent<PlayerController>(out player))
        {
            _stage.EnterStage();
            gameObject.SetActive(false);
        }
    }
}
