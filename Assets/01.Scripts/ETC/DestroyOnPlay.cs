using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnPlay : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject);
    }
}
