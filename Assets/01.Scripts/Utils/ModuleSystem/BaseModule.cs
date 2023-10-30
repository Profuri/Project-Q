using UnityEngine;

namespace ModuleSystem
{
    /// <typeparam name="T">
    /// Base Module Controller
    /// </typeparam>
    public abstract class BaseModule<T> : MonoBehaviour, IModule where T : BaseModuleController
    {
        private T _controller;
        public T Controller => _controller;
        
        public virtual void Init(Transform root)
        {
            _controller = root.GetComponent<T>();
            _controller.OnUpdateEvent += UpdateModule;
            _controller.OnFixedUpdateEvent += FixedUpdateModule;
            _controller.OnDisableEvent += DisableModule;
        }

        public virtual void DisableModule()
        {
            _controller.OnUpdateEvent -= UpdateModule;
            _controller.OnFixedUpdateEvent -= FixedUpdateModule;
            _controller.OnDisableEvent -= DisableModule;
        }
        
        public abstract void UpdateModule();
        public abstract void FixedUpdateModule();
    }
}