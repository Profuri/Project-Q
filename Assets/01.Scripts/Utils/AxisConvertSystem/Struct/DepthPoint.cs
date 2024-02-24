using UnityEngine;

namespace AxisConvertSystem
{
    public struct DepthPoint
    {
        public Vector2 Min;
        public Vector2 Max;

        public float Z;
        
        public bool Block(DepthPoint other)
        {
            return Z < other.Z && other.Min.x <= Min.x && other.Min.y <= Min.y &&
                   other.Max.x >= Max.x && other.Max.y >= Max.y;
        }
    }
}