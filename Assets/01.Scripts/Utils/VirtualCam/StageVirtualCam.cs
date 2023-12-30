using UnityEngine;

namespace VirtualCam
{
    public class StageVirtualCam : VirtualCamComponent
    {
        [SerializeField] private bool _isControlCam;
        public bool IsControlCam => _isControlCam;
        
        [SerializeField] private EAxisType _axisType;
        public EAxisType AxisType => _axisType;
    }
}