using System;
using Cinemachine;
using UnityEngine;

namespace VirtualCam
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public sealed class VirtualCamComponent : MonoBehaviour, IVirtualCam
    {
        private CinemachineVirtualCameraBase _virtualCam;
        [SerializeField] private EAxisType _type;

        public CinemachineVirtualCameraBase VirtualCam => _virtualCam;
        public EAxisType Type => _type;

        public event Action OnEnter = null;
        public event Action OnUpdate = null;
        public event Action OnExit = null;

        private void Awake()
        {
            _virtualCam = GetComponent<CinemachineVirtualCameraBase>();
        }

        public void EnterCam()
        {
            _virtualCam.m_Priority = 10;
            OnEnter?.Invoke();
        }

        public void UpdateCam()
        {
            OnUpdate?.Invoke();
        }

        public void ExitCam()
        {
            _virtualCam.m_Priority = 0;
            OnExit?.Invoke();
        }
    }
}