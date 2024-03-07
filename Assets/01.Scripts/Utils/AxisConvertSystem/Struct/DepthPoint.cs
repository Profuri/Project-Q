using UnityEngine;

namespace AxisConvertSystem
{
    public struct DepthPoint
    {
        public Vector2 Min;
        public Vector2 Max;

        public float Z;

        public bool IsTrigger;
        
        public bool Block(DepthPoint other)
        {
            if (other.IsTrigger)
            {
                return false;
            }

            Debug.Log($"Min: {Min} Max: {Max}");
            //상대가 더 크냐
            //블락이 되면 상대가 더 크다는거지
            //근데 블락이 됐어 내가 더 큰데
            return Z < other.Z && other.Min.x <= Min.x && other.Min.y <= Min.y &&
                   other.Max.x >= Max.x && other.Max.y >= Max.y;
        }

        public bool Block(DepthPoint other,ObjectUnit obj)
        {
            if (other.IsTrigger)
            {
                return false;
            }

            Debug.Log($"Unit Name: {obj.gameObject.name}");
            Debug.Log($"Min: {Min} Max: {Max}");
            //상대가 더 크냐
            //블락이 되면 상대가 더 크다는거지
            //근데 블락이 됐어 내가 더 큰데
            return Z < other.Z && other.Min.x <= Min.x && other.Min.y <= Min.y &&
                   other.Max.x >= Max.x && other.Max.y >= Max.y;
        }
    }
}