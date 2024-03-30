using System;
using System.Collections;
using UnityEngine;

public class ChapterInfoPanel : UIComponent
{
    [SerializeField] private float _lifeTime;

    private WaitForSecondsRealtime _lifeWaitTime;

    protected override void Awake()
    {
        base.Awake();
        _lifeWaitTime = new WaitForSecondsRealtime(_lifeTime);
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, () => StartCoroutine(LifeCycleRoutine()));
    }

    private IEnumerator LifeCycleRoutine()
    {
        yield return _lifeWaitTime;
        Disappear();
    }
}