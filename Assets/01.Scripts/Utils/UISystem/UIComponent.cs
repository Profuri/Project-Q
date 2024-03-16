using UnityEngine;

public class UIComponent : PoolableMono
{
    public UIComponentTweenData tweenData;
    
    public void Appear(Transform canvasTrm, bool useEnterAnim = false)
    {
        transform.SetParent(canvasTrm);
        if (useEnterAnim)
        {
            tweenData.appearAnimator.Play();
        }
    }

    public void Disappear(bool useExitAnim = false)
    {
        if (useExitAnim)
        {
            tweenData.disappearAnimator.Play(() =>
            {
                PoolManager.Instance.Push(this);
            });
            return;
        }
        PoolManager.Instance.Push(this);
    }

    public override void OnPop()
    {
    }

    public override void OnPush()
    {
    }
}