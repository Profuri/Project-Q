using UnityEngine;
using System;

public class MemoWindow : UIComponent
{
    public override void Appear(Transform parentTrm, Action<UIComponent> callback = null)
    {
        base.Appear(parentTrm, callback);
        CursorManager.RegisterUI(this);
    }

    public override void Disappear(Action<UIComponent> callback = null)
    {
        base.Disappear(callback);
        SoundManager.Instance.PlaySFX("PanelDisappear");
        CursorManager.UnRegisterUI(this);
    }

    public void Close()
    {
        Disappear();
    }
}