using System.Collections.Generic;
using UnityEngine;

#if MODULAR_3D_TEXT
#endif

namespace TinyGiantStudio.Layout
{
    [HelpURL("https://ferdowsur.gitbook.io/layout-system/layout-group/linear-layout-group")]
    [AddComponentMenu("Tiny Giant Studio/Layout/Linear Layout Group (M3D)", 30003)]
    public class LinearLayoutGroup : LayoutGroup
    {
        #region Variable Declaration

        public float spacing = 0;
        public Alignment alignment = Alignment.HorizontalMiddle;
        public Alignment secondaryAlignment = Alignment.VerticleMiddle;

        private bool startLoopFromEnd = true;

        public bool randomizeRotations = false;

        [SerializeField] private Vector3 _minimumRandomRotation = Vector3.zero;

        public Vector3 MinimumRandomRotation
        {
            get { return _minimumRandomRotation; }
            set
            {
                if (_minimumRandomRotation != value)
                {
                    rotationChanged = true;
                }

                _minimumRandomRotation = value;
            }
        }

        public Vector3 maximumRandomRotation = Vector3.zero;
        public bool rotationChanged = false;

        public enum Alignment
        {
            Top,
            VerticleMiddle,
            Bottom,
            Left,
            HorizontalMiddle,
            Right
        }

        #endregion Variable Declaration

        public override void UpdateLayout(int startRepositioningFrom = 0)
        {
            if (TotalActiveChildCount() == 0)
                return;

            if (!Application.isPlaying || alwaysUpdateBounds || TotalActiveChildCount() != bounds.Length)
                bounds = GetAllChildBounds();

            float x = 0;
            float y = 0;

            GetStartingPosition(ref x, ref y, bounds);
            //GetPositionValuesAccordingToSelectedLayout(ref x, ref y, bounds);

            startLoopFromEnd = StartLoopFromEnd();

            if (startLoopFromEnd)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom);
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom);
                }
            }
            rotationChanged = false;
        }

        /// <summary>
        /// Same as UpdateLayout but for combined meshes
        /// </summary>
        /// <param name="meshLayouts"></param>
        /// <returns></returns>
        public override List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts)
        {
            if (meshLayouts.Count == 0)
                return null;

            Bounds[] bounds = GetAllChildBounds(meshLayouts);

            float x = 0;
            float y = 0;

            GetStartingPosition(ref x, ref y, bounds);
            //GetPositionValuesAccordingToSelectedLayout(ref x, ref y, bounds);

            startLoopFromEnd = StartLoopFromEnd();

            if (startLoopFromEnd)
            {
                for (int i = meshLayouts.Count - 1; i >= 0; i--)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i]);
                    if (randomizeRotations) meshLayouts[i].rotation.eulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
            else
            {
                for (int i = 0; i < meshLayouts.Count; i++)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i]);
                    if (randomizeRotations) meshLayouts[i].rotation.eulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
            rotationChanged = false;

            return meshLayouts;
        }

        private MeshLayout SetMeshPosition(ref float x, ref float y, Bounds bound, MeshLayout meshLayout)
        {
            float toAddX = 0;
            float toAddY = 0;

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle)
            {
                toAddY -= (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Top)
            {
                toAddY += (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Left)
            {
                toAddX += (spacing / 2) + (bound.size.x) / 2;
                x -= bound.center.x;
            }
            else if (alignment == Alignment.Right || alignment == Alignment.HorizontalMiddle)
            {
                toAddX -= (spacing / 2) + (bound.size.x) / 2;

                x -= bound.center.x;
            }
            x += toAddX;
            y += toAddY;

            meshLayout.position = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

            //transform.GetChild(i).localPosition = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle || alignment == Alignment.Top)
            {
                y += bound.center.y;
            }
            else if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                x += bound.center.x;
            }
            x += toAddX;
            y += toAddY;

            return meshLayout;
        }

        private void SetChildPosition(ref float x, ref float y, int i, Bounds bound, int startRepositioningFrom)
        {
            float toAddX = 0;
            float toAddY = 0;

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle)
            {
                toAddY -= (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Top)
            {
                toAddY += (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Left)
            {
                toAddX += (spacing / 2) + (bound.size.x) / 2;
                x -= bound.center.x;
            }
            else if (alignment == Alignment.Right || alignment == Alignment.HorizontalMiddle)
            {
                toAddX -= (spacing / 2) + (bound.size.x) / 2;

                x -= bound.center.x;
            }

            x += toAddX;
            y += toAddY;

            Vector3 targetPosition = RemoveNaNErrorIfAny(new Vector3(x, y, 0));
            if (i >= startRepositioningFrom)
                SetLocalPosition(transform.GetChild(i), targetPosition);

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle || alignment == Alignment.Top)
            {
                y += bound.center.y;
            }
            else if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                x += bound.center.x;
            }

            x += toAddX;
            y += toAddY;
        }

        private void SetLocalPosition(Transform target, Vector3 targetPosition)
        {
            if (Application.isPlaying && elementUpdater.module)
            {
                if (!randomizeRotations)
                    elementUpdater.module.UpdateLocalPosition(target, elementUpdater.variableHolders, targetPosition);
                else
                    elementUpdater.module.UpdateLocalTransform(target, elementUpdater.variableHolders, targetPosition, GetRandomQuaternionRotation(MinimumRandomRotation, maximumRandomRotation));
            }
            else
            {
                if (target.localPosition != targetPosition)
                {
                    target.localPosition = targetPosition;

                    if (randomizeRotations)
                    {
                        target.localEulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                    }
                }
                else if (rotationChanged && randomizeRotations)
                {
                    target.localEulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
        }

        private Vector3 GetRandomRotation(Vector3 min, Vector3 max)
        {
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            float z = Random.Range(min.z, max.z);

            return new Vector3(x == float.NaN ? 0 : x, y == float.NaN ? 0 : y, z == float.NaN ? 0 : z);
        }

        private Quaternion GetRandomQuaternionRotation(Vector3 min, Vector3 max)
        {
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            float z = Random.Range(min.z, max.z);

            return Quaternion.Euler(x == float.NaN ? 0 : x, y == float.NaN ? 0 : y, z == float.NaN ? 0 : z);
        }

        private void GetStartingPosition(ref float x, ref float y, Bounds[] bounds)
        {
            switch (alignment)
            {
                case Alignment.Top:
                    y = -spacing / 2; //This neutralizes the spacing for the starting character
                    break;

                case Alignment.VerticleMiddle:
                    for (int i = 0; i < bounds.Length; i++)
                    {
                        if (bounds[i].size == Vector3.zero)
                            continue;

                        y += bounds[i].size.y + spacing;
                    }

                    y /= 2;
                    break;

                case Alignment.Bottom:
                    y = spacing / 2; //This neutralizes the spacing for the last character
                    break;

                case Alignment.Left:
                    x = -spacing / 2;
                    break;

                case Alignment.HorizontalMiddle:
                    for (int i = 0; i < bounds.Length; i++)
                    {
                        if (i < transform.childCount && transform.GetChild(i))
                            if (transform.GetChild(i).GetComponent<LayoutElement>())
                                if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement)
                                    continue;

                        if (bounds[i].size == Vector3.zero)
                            continue;

                        x += bounds[i].size.x + spacing;
                    }

                    x /= 2;
                    break;

                case Alignment.Right:
                    x = (spacing / 2);
                    break;

                default:
                    break;
            }

            if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                if (secondaryAlignment == Alignment.Top)
                    y = (MaxBoundHeight(bounds) / 2);
                //y = (-spacing / 2);
                else if (secondaryAlignment == Alignment.Bottom)
                    y = (-MaxBoundHeight(bounds) / 2);
                //y = spacing / 2;
            }
        }

        float MaxBoundHeight(Bounds[] allBounds)
        {
            float y = 0;
            foreach (Bounds bounds in allBounds)
                if (bounds.size.y > y)
                    y = bounds.size.y;

            return y;
        }

        ///// <summary>
        ///// Legacy. Use GetStartingPosition
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="bounds"></param>
        //private void GetPositionValuesAccordingToSelectedLayout(ref float x, ref float y, Bounds[] bounds)
        //{
        //    if (alignment == Alignment.Bottom)
        //    {
        //        y = spacing / 2;
        //    }
        //    else if (alignment == Alignment.VerticleMiddle)
        //    {
        //        for (int i = 0; i < bounds.Length; i++)
        //        {
        //            if (bounds[i].size == Vector3.zero)
        //                continue;

        //            y += bounds[i].size.y + spacing;
        //        }

        //        y /= 2;
        //    }
        //    else if (alignment == Alignment.Top)
        //    {
        //        y = -spacing / 2;
        //    }
        //    else if (alignment == Alignment.Left)
        //    {
        //        x = -spacing / 2;
        //    }
        //    else if (alignment == Alignment.HorizontalMiddle)
        //    {
        //        for (int i = 0; i < bounds.Length; i++)
        //        {
        //            if (i < transform.childCount && transform.GetChild(i))
        //                if (transform.GetChild(i).GetComponent<LayoutElement>())
        //                    if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement)
        //                        continue;

        //            if (bounds[i].size == Vector3.zero)
        //                continue;

        //            x += bounds[i].size.x + spacing;
        //        }

        //        x /= 2;
        //    }
        //    else if (alignment == Alignment.Right)
        //    {
        //        x = (spacing / 2);
        //    }
        //}

        private bool StartLoopFromEnd()
        {
            if (alignment == Alignment.Top) return true;
            else if (alignment == Alignment.VerticleMiddle) return false;
            else if (alignment == Alignment.Bottom) return false;
            else if (alignment == Alignment.Left) return false;
            else if (alignment == Alignment.HorizontalMiddle) return true;
            else if (alignment == Alignment.Right) return true;

            return false; //this never happens
        }
    }
}