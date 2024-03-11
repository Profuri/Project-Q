using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if MODULAR_3D_TEXT
#endif

namespace TinyGiantStudio.Layout
{
    [HelpURL("https://ferdowsur.gitbook.io/layout-system/layout-group/circular-layout-group")]
    [AddComponentMenu("Tiny Giant Studio/Layout/Circular Layout Group (M3D)", 30003)]
    public class CircularLayoutGroup : LayoutGroup
    {
        public Direction direction;
        public enum Direction
        {
            left,
            right
        }

        /// <summary>
        /// Uses style if set to false
        /// </summary>
        public bool useAngle;
        public Vector3 angle;

        public Style style;
        public enum Style
        {
            style1,
            style2,
            style3,
            style4,
            style5
        }

        public float spread = 360;
        public float radius = 5;
        public float radiusDecreaseRate = 0;

        //private float angle;
        private int totalActiveChildCount;
        private float xPos;
        private float yPos;






        public override void UpdateLayout(int startRepositioningFrom = 0)
        {
            totalActiveChildCount = TotalActiveChildCount();
            if (totalActiveChildCount == 0)
                return;

            if (!Application.isPlaying || alwaysUpdateBounds || bounds == null || TotalActiveChildCount() != bounds.Length)
                bounds = GetAllChildBounds();

            //bounds = GetAllChildBounds();
            float totalSize = TotalYSize(bounds);

            float angle = 0;
            float currentRadius = radius;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    float toAdd = (spread) * (Size(bounds[i], totalSize)) / 2;

                    if (direction == Direction.left)
                        angle += toAdd;
                    else
                        angle -= toAdd;


                    xPos = Mathf.Sin(Mathf.Deg2Rad * angle) * currentRadius;
                    yPos = Mathf.Cos(Mathf.Deg2Rad * angle) * currentRadius;

                    if (i >= startRepositioningFrom)
                    {
                        Vector3 targetPosition = new Vector3(xPos, yPos, 0);
                        Quaternion targetRotation = GetRotation(angle, i);

                        targetPosition = RemoveNaNErrorIfAny(targetPosition);

                        SetLocalPositionRotation(targetPosition, targetRotation, transform.GetChild(i));
                    }

                    if (direction == Direction.left)
                        angle += toAdd;
                    else
                        angle -= toAdd;

                    currentRadius -= radiusDecreaseRate;
                }
            }
        }

        private void SetLocalPositionRotation(Vector3 position, Quaternion rotation, Transform target)
        {
            if (elementUpdater != null)
            {
                if (elementUpdater.module)
                {
                    elementUpdater.module.UpdateLocalTransform(target, elementUpdater.variableHolders, position, rotation);
                    return;
                }
            }

            if (target.localPosition != position)//This is to avoid unnecessary marking things as changed in scene hierarchy
                target.localPosition = position;
            if (target.localRotation != rotation)//This is to avoid unnecessary marking things as changed in scene hierarchy
                target.localRotation = rotation;
        }

        public override List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts)
        {
            if (meshLayouts.Count == 0)
                return null;
            Bounds[] bounds = GetAllChildBounds(meshLayouts);
            float totalSize = TotalYSize(bounds);

            float angle = 0;
            float currentRadius = radius;

            for (int i = 0; i < meshLayouts.Count; i++)
            {
                float toAdd = (spread) * (Size(bounds[i], totalSize)) / 2;

                if (direction == Direction.left)
                    angle += toAdd;
                else
                    angle -= toAdd;


                xPos = Mathf.Sin(Mathf.Deg2Rad * angle) * currentRadius;
                yPos = Mathf.Cos(Mathf.Deg2Rad * angle) * currentRadius;

                meshLayouts[i].position = new Vector3(xPos, yPos, 0);
                meshLayouts[i].rotation = GetRotation(angle, i);

                if (direction == Direction.left)
                    angle += toAdd;
                else
                    angle -= toAdd;

                currentRadius -= radiusDecreaseRate;
            }

            return meshLayouts;
        }

        private float Size(Bounds myBound, float totalBound)
        {
            return myBound.size.y / totalBound;
        }

        float TotalYSize(Bounds[] bounds)
        {
            float y = 0;

            for (int i = 0; i < bounds.Length; i++)
            {
                y += bounds[i].size.y;
            }

            return y;
        }



        private Quaternion GetRotation(float angle, int i)
        {
            if (!useAngle)
                return GetRotationFromStyle(angle, i);
            else
                return GetRotationFromFlatRotation(angle);
        }
        private Quaternion GetRotationFromFlatRotation(float rotation)
        {
            return Quaternion.Euler(rotation - angle.x, angle.y, angle.z);
        }
        private Quaternion GetRotationFromStyle(float angle, int i)
        {
            if (float.IsNaN(angle))
                angle = 0;

            //centered
            if (style == Style.style1)
                return Quaternion.Euler(angle - 90, 90, 0);
            else if (style == Style.style2)
                return Quaternion.Euler(angle + 90, 90, 0);
            else if (style == Style.style3)
                return Quaternion.Euler(angle + 90, 90, 90);
            else if (style == Style.style4)
                return Quaternion.Euler(angle - 90, 90, 90);
            else
            {
                Vector3 toTargetVector;
                if (transform.childCount > i)
                    toTargetVector = Vector3.zero - transform.GetChild(i).localPosition;
                else
                    toTargetVector = Vector3.zero;

                float zRotation = Mathf.Atan2(toTargetVector.y, toTargetVector.x) * Mathf.Rad2Deg;
                return Quaternion.Euler(new Vector3(0, 0, zRotation));
            }
        }

        public void SetDirectionLeft()
        {
            direction = Direction.left;
            Debug.Log(direction);
        }

        public void SetDirectionRight()
        {
            direction = Direction.right;
            Debug.Log(direction);

        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0.75f, 0.75f, 1f, 0.9f);

            int totalItems = Mathf.RoundToInt(radius * 10);
            if (totalItems < 10)
                totalItems = 10;

            float gizmoAngle = -(spread / 2);
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i <= totalItems; i++)
            {
                float xPos = Mathf.Sin(Mathf.Deg2Rad * gizmoAngle) * radius;
                float yPos = Mathf.Cos(Mathf.Deg2Rad * gizmoAngle) * radius;

                Vector3 newPos = new Vector3(xPos, yPos, 0);
                points.Add(newPos);

                gizmoAngle += (spread / totalItems);

            }
            Handles.DrawAAPolyLine(5, points.ToArray());
        }
#endif
    }
}