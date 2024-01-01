using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSplash : MonoBehaviour
{
    public float lifeTime = 1f;


    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

}
