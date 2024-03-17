using UnityEngine;

public class UIComponent : PoolableMono
{
    public UIComponentTweenData tweenData;
    public UIComponentType componentType;
    
    public Transform ParentTrm { get; private set; }
    
    public void Appear(Transform parentTrm)
    {
        ParentTrm = parentTrm;
        transform.SetParent(parentTrm);
        tweenData.appearAnimator.Play();
    }

    public void Disappear()
    {
        tweenData.disappearAnimator.Play(() =>
        {
            PoolManager.Instance.Push(this);
        });
    }

    public override void OnPop()
    {
    }

    public override void OnPush()
    {
    }
}