using System;
using UnityEngine;
using UnityEngine.UI;

public class UIComponent : PoolableMono
{
    public UIComponentTweenData tweenData;
    public UIComponentType componentType;
    
    public Transform ParentTrm { get; private set; }

    protected virtual void Awake()
    {
        if (tweenData)
        {
            tweenData.appearAnimator.Init(this);
            tweenData.disappearAnimator.Init(this);
        }
    }

    public virtual void Appear(Transform parentTrm)
    {
        ParentTrm = parentTrm;
        transform.SetParent(parentTrm);
        if (tweenData)
        {
            tweenData.appearAnimator.Play();
        }
    }

    public virtual void Disappear()
    {
        if (tweenData)
        {
            tweenData.disappearAnimator.Play(() =>
            {
                PoolManager.Instance.Push(this);
            });
        }
        else
        {
            PoolManager.Instance.Push(this);
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (transform is RectTransform rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            var adjustPos = UIManager.Instance.AdjustUIRectPosition(position, rectTransform.rect);
            rectTransform.anchoredPosition = adjustPos;
        }
        else
        {
            transform.localPosition = position;
        }
    }

    public override void OnPop()
    {
    }

    public override void OnPush()
    {
    }
}