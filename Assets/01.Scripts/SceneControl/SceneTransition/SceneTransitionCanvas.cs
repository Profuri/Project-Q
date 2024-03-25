using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class SceneTransitionCanvas : PoolableMono
{
    [SerializeField] private RectTransform _circleRectTrm;
    private Coroutine _transitionCor;
    private Coroutine _delayCor;

    public static Vector2 sMaxSize = new Vector2(2200f, 2200f);

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
    public void PresentTransition(Vector2 startWidth, Vector2 endWidth, float time, Action Callback = null)
    {
        if(_transitionCor != null)
        {
            StopCoroutine(_transitionCor);
            _transitionCor = null;
        }
        _transitionCor = StartCoroutine(TransitionCoroutine(startWidth, endWidth, time, Callback));

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

    private IEnumerator TransitionCoroutine(Vector2 startWidth, Vector2 endWidth, float time, Action Callback = null)
    {
        float timer = 0f;
        float percent = timer / time;

        Vector2 sizeDelta = Vector2.Lerp(startWidth, endWidth, percent);

        _circleRectTrm.sizeDelta = sizeDelta;
        while(percent <= 0.99f)
        {
            timer += Time.deltaTime;
            percent = timer / time;
            sizeDelta = Vector2.Lerp(startWidth, endWidth, percent);
            _circleRectTrm.sizeDelta = sizeDelta;
            yield return null;
        }

        _circleRectTrm.sizeDelta = endWidth;
        Callback?.Invoke();
    }
}
