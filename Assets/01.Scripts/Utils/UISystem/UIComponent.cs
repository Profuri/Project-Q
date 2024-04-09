using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIComponent : PoolableMono
{
    public UIComponentTweenData tweenData;
    
    public Transform ParentTrm { get; private set; }
    public bool IsTweening { get; private set; }
    
    protected virtual void Awake()
    {
        if (tweenData)
        {
            tweenData.appearAnimator.Init(this);
            tweenData.disappearAnimator.Init(this);
        }
    }

    public virtual void Appear(Transform parentTrm, Action callback = null)
    {
        ParentTrm = parentTrm;
        transform.SetParent(parentTrm);
        transform.localRotation = quaternion.identity;
        transform.localScale = Vector3.one;

        var localPos = transform.localPosition;
        localPos.z = 0;
        transform.localPosition = localPos;  
        
        if (tweenData)
        {
            IsTweening = true;
            callback += () => IsTweening = false;
            tweenData.appearAnimator?.Play(callback);
        }
    }

    public virtual void Disappear(Action callback = null)
    {
        if (tweenData)
        {
            IsTweening = true;
            tweenData.disappearAnimator.Play(() =>
            {
                callback?.Invoke();
                IsTweening = false;
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
        //Disappear();
    }
}