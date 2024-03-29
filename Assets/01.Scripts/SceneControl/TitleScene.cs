using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : Scene
{
    public override void OnPop()
    {
        base.OnPop();
        //일단 야매로 고침
        if(CameraManager.Instance != null)
        {
            CameraManager.Instance?.CurrentCamController?.CurrentSelectedCam?.SetFollowTarget(null);
        }
    }
}
