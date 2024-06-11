using System.Collections;
using AxisConvertSystem;
using UnityEngine;

namespace VirtualCam
{
    public class SectionVirtualCam : VirtualCamComponent
    {
        [SerializeField] private bool _isControlCam;
        public bool IsControlCam => _isControlCam;
        
        [SerializeField] private AxisType _axisType;
        public AxisType AxisType => _axisType;
    }
}