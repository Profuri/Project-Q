using TinyGiantStudio.Modules;
using UnityEngine;
using UnityEngine.AI;

namespace TinyGiantStudio.Layout
{
    [DisallowMultipleComponent]
    public class LayoutElementNavmeshController : MonoBehaviour
    {
        //bool settingPosition = false;
        public Vector3 startingPosition;
        public Vector3 targetPosition;

        public float gravitationalAcceleration = 10;

        void Update()
        {
            if (transform.position != targetPosition)
            {
                ////GetComponent<Rigidbody>().AddForce((targetPosition - transform.position) * gravitationalAcceleration * Time.deltaTime);
                //Vector3 direction = targetPosition - transform.position;
                //GetComponent<Rigidbody>().AddForceAtPosition(direction.normalized, transform.position);
            }

            //if (settingPosition)
            //{
            //    //if (timer < moveDuration)
            //    //{
            //    //    float perc = timer / moveDuration;
            //    //    GetComponent<Rigidbody>().velocity += gravitationalAcceleration * Time.fixedTime * (targetPosition - transform.position);
            //    //    transform.localPosition = Vector3.LerpUnclamped(startingPosition, targetPosition, positionAnimationCurve.Evaluate(perc));
            //    //}
            //    //else
            //    //{
            //    //    transform.localPosition = targetPosition;
            //    //    settingPosition = false;
            //    //}
            //}

            //if (settingRotation)
            //{
            //    if (timer < rotateDuration)
            //    {
            //        if (!IsNaN(startingRotation) && !IsNaN(targetRotation))
            //        {
            //            float perc = timer / moveDuration;
            //            transform.localRotation = Quaternion.LerpUnclamped(startingRotation, targetRotation, rotationAnimationCurve.Evaluate(perc)); ;
            //        }
            //    }
            //    else
            //    {
            //        transform.localRotation = targetRotation;
            //        settingRotation = false;
            //    }
            //}

            //if (!settingRotation && !settingPosition)
            //    this.enabled = false;


            //timer += Time.deltaTime;
        }

        public void NewTargetLocalPosition(VariableHolder[] variableHolders, Vector3 target)
        {
            if (targetPosition == target)
                return;

            NavMesh.avoidancePredictionTime = 2.5f;
            NavMesh.pathfindingIterationsPerFrame = 1000;


            //GetComponent<NavMeshAgent>().avoidancePriority = Random.Range(50, 100);
            GetComponent<NavMeshAgent>().obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            GetComponent<NavMeshAgent>().SetDestination(targetPosition);
        }
        public void NewTargetLocalRotation(VariableHolder[] variableHolders, Quaternion target)
        {
            //if (targetRotation == target)
            //    return;

            //timer = 0;
            //startingRotation = transform.localRotation;
            //targetRotation = target;
            //rotateDuration = variableHolders[2].floatValue;
            //rotationAnimationCurve = variableHolders[3].animationCurve;

            //settingRotation = true;

            //this.enabled = true;
        }

        private bool IsNaN(Quaternion q)
        {
            return q.x == 0 && q.y == 0 && q.z == 0;
        }
    }
}