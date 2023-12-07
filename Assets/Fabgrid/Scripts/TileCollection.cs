using UnityEngine;

namespace Fabgrid
{
    [System.Serializable]
    public class TileCollection
    {
        [SerializeField]
        public GameObject[] tiles = new GameObject[0];
    }
}
