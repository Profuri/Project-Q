using VirtualCam;

public class PlayerCamController : VirtualCamController
{
    public override void Init()
    {
        base.Init();
        CurrentSelectedCam = _virtualCams[0];
        SetCurrentCam();
    }

    public void SetPlayer(PlayerController player)
    {
        CurrentSelectedCam.SetFollowTarget(player.transform);
    }
}