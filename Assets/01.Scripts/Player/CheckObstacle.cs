using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObstacle : MonoBehaviour
{
    private int _layer = LayerMask.NameToLayer("Obstacle");
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == _layer)
        {
            Restart();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == _layer)
        {
            Restart();
        }
    }


    void Restart()
    {
        // 다시 시작
    }

    
}
