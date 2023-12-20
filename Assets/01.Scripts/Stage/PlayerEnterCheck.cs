using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterCheck : MonoBehaviour
{
    Stage _stage;
    Vector3 _restartPos;
    private void Awake()
    {
        _stage = transform.parent.GetComponent<Stage>();
        Transform restartTrm = transform.Find("RestartPos");
        if (restartTrm != null)
        {

            _restartPos = restartTrm.position- transform.parent.position;
            Debug.Log($"{transform.parent.gameObject.name} : {_restartPos}");
        }
        else
        {
            Debug.Log($"{transform.parent.gameObject.name} : RestartPos ÁöÁ¤ÇØ");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player;
        if(other.TryGetComponent<PlayerController>(out player))
        {
            _stage.EnterStage(_restartPos);
            gameObject.SetActive(false);
        }
    }
}
