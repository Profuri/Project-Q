using System.Collections;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public class CoroutineManager : BaseManager<CoroutineManager>
{
    private Dictionary<string, Coroutine> _coroutineDiction;

    public override void Init()
    {
        base.Init();
        _coroutineDiction = new Dictionary<string, Coroutine>();
    }

    public override void StartManager()
    {
        _coroutineDiction.Clear();
    }

    public new void StartCoroutine(IEnumerator routine)
    {
        var routineName = routine.ToString();
        
        if (_coroutineDiction.ContainsKey(routineName) && _coroutineDiction[routineName] is not null)
        {
            StopCoroutine(routineName);
        }
        
        base.StartCoroutine(CoroutinePlayRoutine(routineName, routine));
    }

    public new void StopCoroutine(IEnumerator routine)
    {
        var routineName = routine.ToString();
        base.StopCoroutine(_coroutineDiction[routineName]);
        _coroutineDiction[routineName] = null;
    }

    public new void StopCoroutine(string routineName)
    {
        base.StopCoroutine(_coroutineDiction[routineName]);
        _coroutineDiction[routineName] = null;
    }

    private IEnumerator CoroutinePlayRoutine(string routineName, IEnumerator routine)
    {
        _coroutineDiction[routineName] = base.StartCoroutine(routine);
        yield return _coroutineDiction[routineName];
        _coroutineDiction[routineName] = null;
    }
}