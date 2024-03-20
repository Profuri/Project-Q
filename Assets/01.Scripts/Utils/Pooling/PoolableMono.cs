using UnityEngine;


public abstract class PoolableMono : MonoBehaviour
{
    public int poolingCnt = 5;

    public abstract void OnPop();
    public abstract void OnPush();
}