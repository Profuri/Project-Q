using UnityEngine;


public abstract class PoolableMono : ExtendedMono
{
    public int poolingCnt = 5;
    [HideInInspector] public bool poolOut = false;

    public abstract void OnPop();
    public abstract void OnPush();
}