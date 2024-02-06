using UnityEngine;

public abstract class PoolableMono : MonoBehaviour
{
    public abstract void OnPop();
    public abstract void OnPush();
}