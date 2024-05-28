using VirtualCam;
using UnityEngine;

public class PlayerCamController : VirtualCamController
{
    private Vector3 _originPos;
    private Quaternion _originRot;
    
    public override void Init()
    {
        base.Init();
        CurrentSelectedCam = _virtualCams[0];
        _originPos = CurrentSelectedCam.transform.position;
        _originRot = CurrentSelectedCam.transform.rotation;
    }

    public override void ResetCamera()
    {
        _virtualCams.ForEach(cam => cam.SetFollowTarget(null));
        CurrentSelectedCam.transform.position = _originPos;
        CurrentSelectedCam.transform.rotation = _originRot;
    }

    public void SetPlayer(PlayerUnit player)
    {
        CurrentSelectedCam.SetFollowTarget(player.transform);
    }
}