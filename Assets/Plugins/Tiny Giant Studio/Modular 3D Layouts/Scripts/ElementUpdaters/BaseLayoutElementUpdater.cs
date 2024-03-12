using TinyGiantStudio.Modules;
using UnityEngine;

namespace TinyGiantStudio.Layout
{
    [CreateAssetMenu(menuName = "Tiny Giant Studio/Modular 3D Layouts/Element Updater/Flat")]
    public class BaseLayoutElementUpdater : LayoutElementModule
    {
        public override void UpdateLocalTransform(Transform transform, VariableHolder[] variableHolders, Vector3 localPosition, Quaternion localRotation)
        {
            if (transform.localPosition != localPosition) //This is to avoid unnecessary marking things as changed in scene hierarchy
                transform.localPosition = localPosition;

            if (transform.localRotation != localRotation) //This is to avoid unnecessary marking things as changed in scene hierarchy
                transform.localRotation = localRotation;
        }
        public override void UpdateWorldTransform(Transform transform, VariableHolder[] variableHolders, Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }
        public override void UpdateLocalPosition(Transform transform, VariableHolder[] variableHolders, Vector3 localPosition)
        {
            if (transform.localPosition != localPosition) //This is to avoid unnecessary marking things as changed in scene hierarchy
                transform.localPosition = localPosition;
        }
        public override void UpdateWorldPosition(Transform transform, VariableHolder[] variableHolders, Vector3 position)
        {
            transform.position = position;
        }
        public override void UpdateLocalRotation(Transform transform, VariableHolder[] variableHolders, Quaternion localRotation)
        {
            if (transform.localRotation != localRotation) //This is to avoid unnecessary marking things as changed in scene hierarchy
                transform.localRotation = localRotation;
        }
        public override void UpdateWorldRotation(Transform transform, VariableHolder[] variableHolders, Quaternion rotation)
        {
            transform.rotation = rotation;
        }








        public override string VariableWarnings(VariableHolder[] variableHolders)
        {
            return null;
        }
    }
}