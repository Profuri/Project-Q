using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : Scene
{
    public override void OnPop()
    {
        base.OnPop();

        //CameraManager.Instance.InitCamera();
        CursorManager.ForceEnableCursor(true);
    }

    public override void OnPush()
    {
        base.OnPush();

        CursorManager.ForceEnableCursor(false);
    }
}
