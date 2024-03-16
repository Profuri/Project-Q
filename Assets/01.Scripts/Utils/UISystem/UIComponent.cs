using UnityEngine;

public class UIComponent : PoolableMono
{
    public UIAnimator appearAnimator;
    public UIAnimator disappearAnimator;
    
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