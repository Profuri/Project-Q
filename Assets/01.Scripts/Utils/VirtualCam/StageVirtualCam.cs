using UnityEngine;

namespace VirtualCam
{
    public class StageVirtualCam : VirtualCamComponent
    {
        [SerializeField] private EAxisType _axisType;
        public EAxisType AxisType => _axisType;
    }
}