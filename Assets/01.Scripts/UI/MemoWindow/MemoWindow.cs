using UnityEngine;
using System;

public class MemoWindow : UIComponent
{
    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, callback);
        
        CursorManager.RegisterUI(this);
    }

    public override void Disappear(Action callback = null)
    {
        base.Disappear(callback);

        CursorManager.UnRegisterUI(this);
    }

    public void Close()
    {
        Disappear();
    }
}