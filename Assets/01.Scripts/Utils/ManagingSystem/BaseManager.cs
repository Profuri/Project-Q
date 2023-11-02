using System;
using Singleton;
using UnityEngine;

namespace ManagingSystem
{
    public abstract class BaseManager<T> : MonoSingleton<T>, IManager where T : MonoBehaviour
    {
        public virtual void Awake()
        {
            GameManager.Instance.OnStartEvent += StartManager;
            GameManager.Instance.OnUpdateEvent += UpdateManager;
        }

        public abstract void StartManager();
        public abstract void UpdateManager();
    }
}