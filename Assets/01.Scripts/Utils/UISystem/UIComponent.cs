using System;
using UnityEngine;

public class UIComponent : PoolableMono
{
    public UIComponentTweenData tweenData;
    public UIComponentType componentType;
    
    public Transform ParentTrm { get; private set; }

    protected virtual void Awake()
    {
        if (tweenData is not null)
        {
            tweenData.appearAnimator.Init(this);
            tweenData.disappearAnimator.Init(this);
        }
    }

    public virtual void Appear(Transform parentTrm)
    {
        ParentTrm = parentTrm;
        transform.SetParent(parentTrm);
        tweenData.appearAnimator.Play();
    }

    public virtual void Disappear()
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