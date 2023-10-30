using UnityEngine;

namespace Core
{
    public static class Define
    {
        private static Camera _mainCam;
        public static Camera MainCam
        {
            get
            {
                if (_mainCam == null)
                {
                    _mainCam = Camera.main;
                }

                return _mainCam;
            }
        }
    }
}

