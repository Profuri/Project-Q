using System;
using Singleton;
using UnityEngine;

namespace ManagingSystem
{
    public abstract class BaseManager<T> : MonoSingleton<T>, IManager where T : MonoBehaviour
    {
        public virtual void Init()
        {
            GameManager.Instance.OnStartEvent += StartManager;
        }

        public abstract void StartManager();
    }
}