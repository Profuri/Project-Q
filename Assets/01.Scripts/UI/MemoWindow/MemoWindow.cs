using UnityEngine;
using System;

public class MemoWindow : UIComponent
{
    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, callback);

        CursorManager.SetCursorEnable(true);
        CursorManager.SetCursorLockState(CursorLockMode.None);

    }

    public void Close()
    {
        Disappear();
    }

    public override void Disappear(Action callback = null)
    {
        base.Disappear(callback);

        CursorManager.SetCursorEnable(false);
        CursorManager.SetCursorLockState(CursorLockMode.Locked);
    }
}