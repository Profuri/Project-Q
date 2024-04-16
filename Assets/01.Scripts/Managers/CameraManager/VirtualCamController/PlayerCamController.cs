using VirtualCam;
using UnityEngine;

public class PlayerCamController : VirtualCamController
{
    private Vector3 _originPos;
    public override void Init()
    {
        base.Init();
        CurrentSelectedCam = _virtualCams[0];
        _originPos = CurrentSelectedCam.transform.position;
    }

    public override void ResetCamera()
    {
        _virtualCams.ForEach(cam => cam.SetFollowTarget(null));
        CurrentSelectedCam.transform.position = _originPos;
        
    }

    public void SetPlayer(PlayerUnit player)
    {
        CurrentSelectedCam.SetFollowTarget(player.transform);
    }
}