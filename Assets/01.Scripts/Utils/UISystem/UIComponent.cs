using UnityEngine;

public class UIComponent : PoolableMono
{
    [HideInInspector] public UIAnimator appearAnimator = new UIAnimator();
    [HideInInspector] public UIAnimator disappearAnimator = new UIAnimator();
    
    public void Appear(Transform canvasTrm, bool useEnterAnim = false)
    {
        transform.SetParent(canvasTrm);
    }

    public void Disappear()
    {
        PoolManager.Instance.Push(this);
    }

    public override void OnPop()
    {
    }

    public override void OnPush()
    {
    }
}