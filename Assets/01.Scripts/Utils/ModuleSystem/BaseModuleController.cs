using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModuleSystem
{
    public class BaseModuleController : MonoBehaviour
    {
        private Dictionary<Type, IModule> _modules;

        public event Action OnUpdateEvent = null;
        public event Action OnFixedUpdateEvent = null;
        public event Action OnDisableEvent = null;

        public virtual void Start()
        {
            _modules = new Dictionary<Type, IModule>();
            var modules = transform.GetComponentsInChildren<IModule>();
            foreach (var module in modules)
            {
                _modules.Add(module.GetType(), module);
                module.Init(transform);
            }
        }

        public virtual void Update()
        {
            OnUpdateEvent?.Invoke();
        }

        public void FixedUpdate()
        {
            OnFixedUpdateEvent?.Invoke();
        }

        public void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }

        public T GetModule<T>() where T : IModule
        {
            return (T)_modules[typeof(T)];
        }
    }
}