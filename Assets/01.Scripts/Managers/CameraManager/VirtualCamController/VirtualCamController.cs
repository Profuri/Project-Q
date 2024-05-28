using System;
using System.Collections.Generic;using UnityEngine;
using VirtualCam;

public abstract class VirtualCamController : MonoBehaviour
{
    [SerializeField] protected List<VirtualCamComponent> _virtualCams;
    public VirtualCamComponent CurrentSelectedCam { get; protected set; }

    public virtual void Init()
    {
        foreach (var vCam in _virtualCams)
        {
            vCam.ExitCam();
        }
        CurrentSelectedCam = null;
    }

    public abstract void ResetCamera();
    
    public virtual void SetCurrentCam(Action callBack = null)
    {
        CameraManager.Instance.ChangeCamera(CurrentSelectedCam, callBack);
    }
}