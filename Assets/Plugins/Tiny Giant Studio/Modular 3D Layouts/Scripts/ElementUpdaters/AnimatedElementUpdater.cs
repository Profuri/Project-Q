using TinyGiantStudio.Modules;
using UnityEngine;

namespace TinyGiantStudio.Layout
{
    // 1 = duration
    // 2 = position move curve


    [CreateAssetMenu(menuName = "Tiny Giant Studio/Modular 3D Layouts/Element Updater/Animated")]
    public class AnimatedElementUpdater : LayoutElementModule
    {
        public override void UpdateLocalTransform(Transform transform, VariableHolder[] variableHolders, Vector3 localPosition, Quaternion localRotation)
        {
            if (Application.isPlaying)
            {
                AddAnimatorIfRequired(transform);

                LayoutElementAnimator elementAnimator = transform.gameObject.GetComponent<LayoutElementAnimator>();

                if (elementAnimator.targetPosition != localPosition)
                {
                    elementAnimator.NewTargetLocalPosition(variableHolders, localPosition);

                    //if (elementAnimator.targetRotation != localRotation)
                    elementAnimator.NewTargetLocalRotation(variableHolders, localRotation);
                }
            }
            else
            {
                if (transform.localPosition != localPosition) //This is to avoid unnecessary marking things as changed in scene hierarchy
                    transform.localPosition = localPosition;

                if (transform.localRotation != localRotation) //This is to avoid unnecessary marking things as changed in scene hierarchy
                    transform.localRotation = localRotation;
            }
        }
        public override void UpdateLocalPosition(Transform transform, VariableHolder[] variableHolders, Vector3 localPosition)
        {
            if (transform.localPosition != localPosition) //This is to avoid unnecessary marking things as changed in scene hierarchy
            {
                if (Application.isPlaying)
                {
                    AddAnimatorIfRequired(transform);
                    transform.gameObject.GetComponent<LayoutElementAnimator>().NewTargetLocalPosition(variableHolders, localPosition);
                }
                else
                {
                    transform.localPosition = localPosition;
                }
            }
        }
        public override void UpdateLocalRotation(Transform transform, VariableHolder[] variableHolders, Quaternion localRotation)
        {
            if (transform.localRotation != localRotation) //This is to avoid unnecessary marking things as changed in scene hierarchy
            {
                if (Application.isPlaying)
                {
                    AddAnimatorIfRequired(transform);
                    transform.gameObject.GetComponent<LayoutElementAnimator>().NewTargetLocalRotation(variableHolders, localRotation);
                }
                else
                {
                    transform.localRotation = localRotation;
                }
            }
        }



        public override void UpdateWorldTransform(Transform transform, VariableHolder[] variableHolders, Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }
        public override void UpdateWorldPosition(Transform transform, VariableHolder[] variableHolders, Vector3 position)
        {
            transform.position = position;
        }
        public override void UpdateWorldRotation(Transform transform, VariableHolder[] variableHolders, Quaternion rotation)
        {
            transform.rotation = rotation;
        }


        void AddAnimatorIfRequired(Transform transform)
        {
            if (!transform.gameObject.GetComponent<LayoutElementAnimator>())
                transform.gameObject.AddComponent<LayoutElementAnimator>();
        }




        public override string VariableWarnings(VariableHolder[] variableHolders)
        {
            return null;
        }
    }
}