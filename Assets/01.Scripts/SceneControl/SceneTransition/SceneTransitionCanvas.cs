using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

public class SceneTransitionCanvas : PoolableMono
{
    [SerializeField] private Image _image;
    private Coroutine _transitionCor;
    private Coroutine _delayCor;

    public override void OnPop()
    {

    }

    public override void OnPush()
    {

    }


    /// <summary>
    /// width is circle size, if width is up, circle is bigger;
    /// </summary>
    /// <param name="startWidth"></param>
    /// <param name="endWidth"></param>
    /// <param name="time"></param>
    /// <param name="Callback"></param>
    public void PresentTransition(float startValue, float endValue, float time, Action Callback = null)
    {
        if(_transitionCor != null)
        {
            StopCoroutine(_transitionCor);
            _transitionCor = null;
        }
        _transitionCor = StartCoroutine(TransitionCoroutine(startValue, endValue, time, Callback));

    }

    public void PauseTransition(float time, Action Callback)
    {
        if(_delayCor != null)
        {
            StopCoroutine(_delayCor);
            _delayCor = null;
        }
        _delayCor = StartCoroutine(DelayCoroutine(time, Callback));
    }

    private IEnumerator DelayCoroutine(float time, Action Callback)
    {
        yield return new WaitForSeconds(time);
        Callback?.Invoke();
    }

    private IEnumerator TransitionCoroutine(float startValue, float endValue, float time, Action Callback = null)
    {
        float timer = 0f;
        float percent = timer / time;

        float currentValue = Mathf.Lerp(startValue, endValue, percent);
        Color originColor = _image.color;
        originColor.a = currentValue;
        _image.color = originColor;
        while (percent <= 0.99f)
        {
            timer += Time.deltaTime;
            percent = timer / time;
            currentValue = Mathf.Lerp(startValue, endValue, percent);
            originColor.a = currentValue;
            _image.color = originColor; 
            yield return null;
        }

        Callback?.Invoke();
    }
}
