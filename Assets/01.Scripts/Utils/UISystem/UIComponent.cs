using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIComponent : PoolableMono
{
    public UIComponentTweenData tweenData;
    
    public Transform ParentTrm { get; private set; }
    public bool IsTweening { get; private set; }

    private Vector3 _originLocalPos;
    private Quaternion _originLocalRot;
    private Vector3 _originScale;
    
    protected virtual void Awake()
    {
        _originLocalPos = transform.localPosition;
        _originLocalRot = transform.localRotation;
        _originScale = transform.localScale;
        
        if (tweenData)
        {
            tweenData.appearAnimator.Init(this);
            tweenData.disappearAnimator.Init(this);
        }
    }

    public virtual void Appear(Transform parentTrm, Action<UIComponent> callback = null)
    {
        ParentTrm = parentTrm;
        transform.SetParent(parentTrm);
        transform.localRotation = _originLocalRot;
        transform.localScale = _originScale;

        var localPos = _originLocalPos;
        localPos.z = 0;
        transform.localPosition = localPos;  
        
        if (tweenData)
        {
            IsTweening = true;
            tweenData.appearAnimator?.Play(() =>
            {
                IsTweening = false;
                callback?.Invoke(this);
            });
        }
    }

    public virtual void Disappear(Action<UIComponent> callback = null)
    {
        if (tweenData)
        {
            IsTweening = true;
            tweenData.disappearAnimator.Play(() =>
            {
                callback?.Invoke(this);
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

    public void ResetPosition()
    {
        if(transform is RectTransform rectTransform)
        {
            rectTransform.offsetMin = Vector3.zero;
            rectTransform.offsetMax = Vector3.zero;
        }
        else
        {
            transform.position = Vector3.zero;
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