using TinyGiantStudio.Modules;
using UnityEngine;

namespace TinyGiantStudio.Layout
{
    public abstract class LayoutElementModule : ModuleCore
    {
        public abstract void UpdateLocalTransform(Transform target, VariableHolder[] variableHolders, Vector3 localPosition, Quaternion localRotation);
        public abstract void UpdateWorldTransform(Transform target, VariableHolder[] variableHolders, Vector3 position, Quaternion rotation);
        public abstract void UpdateLocalPosition(Transform target, VariableHolder[] variableHolders, Vector3 localPosition);
        public abstract void UpdateWorldPosition(Transform target, VariableHolder[] variableHolders, Vector3 position);
        public abstract void UpdateLocalRotation(Transform target, VariableHolder[] variableHolders, Quaternion localRotation);
        public abstract void UpdateWorldRotation(Transform target, VariableHolder[] variableHolders, Quaternion rotation);
    }
}