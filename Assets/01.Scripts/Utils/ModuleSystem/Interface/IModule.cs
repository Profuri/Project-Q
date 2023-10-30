using UnityEngine;

namespace ModuleSystem{
    public interface IModule
    {
        /// <summary>
        /// Initialize module when call "Awake"
        /// </summary>
        /// <param name="root">
        /// Base transform that contain ModuleController
        /// </param>
        public void Init(Transform root);
        
        /// <summary>
        /// Updating module when call "Update"
        /// </summary>
        public void UpdateModule();
        
        /// <summary>
        /// FixedUpdating module when call "FixedUpdate"
        /// </summary>
        public void FixedUpdateModule();
        
        /// <summary>
        /// Disable module when call "OnDisable"
        /// </summary>
        public void DisableModule();
    }
}