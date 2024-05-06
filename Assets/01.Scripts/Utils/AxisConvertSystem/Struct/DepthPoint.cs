using UnityEngine;

namespace AxisConvertSystem
{
    public struct DepthPoint
    {
        public Vector2 Min;
        public Vector2 Max;

        public float Z;

        public bool IsTrigger;

        public bool Intersect(DepthPoint other)
        {
            if (other.IsTrigger)
            {
                return false;
            }
            
            var isInsideOther = IsInsidePoint(Min, other.Min, other.Max) ||
                               IsInsidePoint(Max, other.Min, other.Max) ||
                               IsInsidePoint(new Vector2(Min.x, Max.y), other.Min, other.Max) ||
                               IsInsidePoint(new Vector2(Max.x, Min.y), other.Min, other.Max);
            
            var isInsideThis = IsInsidePoint(other.Min, Min, Max) ||
                              IsInsidePoint(other.Max, Min, Max) ||
                              IsInsidePoint(new Vector2(other.Min.x, other.Max.y), Min, Max) ||
                              IsInsidePoint(new Vector2(other.Max.x, other.Min.y), Min, Max);

            return isInsideOther || isInsideThis;
        }

        private bool IsInsidePoint(Vector2 point, Vector2 min, Vector2 max)
        {
            return point.x >= min.x && point.x <= max.x &&
                   point.y >= min.y && point.y <= max.y;
        }

        public bool Block(DepthPoint other)
        {
            if (other.IsTrigger)
            {
                return false;
            }
            
            return Z < other.Z && other.Min.x <= Min.x && other.Min.y <= Min.y &&
                   other.Max.x >= Max.x && other.Max.y >= Max.y;
        }
    }
}