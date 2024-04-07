using VirtualCam;
using UnityEngine;

public class PlayerCamController : VirtualCamController
{

    public override void Init()
    {
        base.Init();
        CurrentSelectedCam = _virtualCams[0];
    }

    public override void ResetCamera()
    {
        _virtualCams.ForEach(cam => cam.SetFollowTarget(null));
    }

    public void SetPlayer(PlayerUnit player)
    {
        CurrentSelectedCam.SetFollowTarget(player.transform);
    }
}