using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : Scene
{
    public override void OnPop()
    {
        base.OnPop();
        //�ϴ� �߸ŷ� ��ħ
        //if(CameraManager.Instance != null)
        //{
         //   CameraManager.Instance?.CurrentCamController?.CurrentSelectedCam?.SetFollowTarget(null);
        //}

        CursorManager.ForceEnableCursor(true);
    }

    public override void OnPush()
    {
        base.OnPush();

        CursorManager.ForceEnableCursor(false);
    }
}
