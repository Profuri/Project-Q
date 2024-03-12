using TinyGiantStudio.Modules;
using UnityEngine;

namespace TinyGiantStudio.Layout
{
    [DisallowMultipleComponent]
    public class LayoutElementAnimator : MonoBehaviour
    {
        bool settingPosition = false;
        public float moveDuration;
        public Vector3 startingPosition;
        public Vector3 targetPosition;
        public AnimationCurve positionAnimationCurve;

        bool settingRotation = false;
        public float rotateDuration;
        public Quaternion startingRotation;
        public Quaternion targetRotation;
        public AnimationCurve rotationAnimationCurve;

        public float positionTimer = 0;
        public float rotationTimer = 0;

        void Update()
        {
            if (settingPosition)
            {
                if (positionTimer < moveDuration)
                {
                    float perc = positionTimer / moveDuration;
                    transform.localPosition = Vector3.LerpUnclamped(startingPosition, targetPosition, positionAnimationCurve.Evaluate(perc));
                }
                else
                {
                    transform.localPosition = targetPosition;
                    settingPosition = false;
                }
            }

            if (settingRotation)
            {
                if (rotationTimer < rotateDuration)
                {
                    if (!IsNaN(startingRotation) && !IsNaN(targetRotation))
                    {
                        float perc = rotationTimer / moveDuration;
                        var currentRotation = Quaternion.LerpUnclamped(startingRotation, targetRotation, rotationAnimationCurve.Evaluate(perc));
                        if (!IsNaN(currentRotation))
                            transform.localRotation = currentRotation;
                    }
                }
                else
                {
                    transform.localRotation = targetRotation;
                    settingRotation = false;
                }
            }

            if (!settingRotation && !settingPosition)
                this.enabled = false;


            positionTimer += Time.deltaTime;
            rotationTimer += Time.deltaTime;
        }

        public void NewTargetLocalPosition(VariableHolder[] variableHolders, Vector3 newTargetPosition)
        {
            if (targetPosition == newTargetPosition)
                return;

            targetPosition = newTargetPosition;
            positionTimer = 0;
            startingPosition = transform.localPosition;
            moveDuration = variableHolders[0].floatValue;
            positionAnimationCurve = variableHolders[1].animationCurve;

            settingPosition = true;

            this.enabled = true;
        }
        public void NewTargetLocalRotation(VariableHolder[] variableHolders, Quaternion target)
        {
            if (targetRotation == target)
                return;

            rotationTimer = 0;
            startingRotation = transform.localRotation;
            targetRotation = target;
            rotateDuration = variableHolders[2].floatValue;
            rotationAnimationCurve = variableHolders[3].animationCurve;

            settingRotation = true;

            this.enabled = true;
        }

        private bool IsNaN(Quaternion q)
        {
            //return q.x == 0 && q.y == 0 && q.z == 0;
            return float.IsNaN(q.x) && float.IsNaN(q.y) && float.IsNaN(q.z);
        }

    }
}