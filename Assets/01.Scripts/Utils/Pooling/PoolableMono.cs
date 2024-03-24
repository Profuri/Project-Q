using UnityEngine;


public abstract class PoolableMono : MonoBehaviour
{
    public int poolingCnt = 5;
    [HideInInspector] public bool poolOut = false;

    public abstract void OnPop();
    public abstract void OnPush();
}