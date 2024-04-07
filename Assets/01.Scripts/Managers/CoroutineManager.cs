using System.Collections;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public class CoroutineManager : BaseManager<CoroutineManager>
{
    private Dictionary<int, Dictionary<string, Coroutine>> _coroutineDiction;

    public override void Init()
    {
        base.Init();
        _coroutineDiction = new Dictionary<int, Dictionary<string, Coroutine>>();
    }

    public override void StartManager()
    {
        _coroutineDiction.Clear();
    }

    public Coroutine StartSafeCoroutine(int ownerInstanceId, IEnumerator routine)
    {
        var routineName = routine.ToString();

        if (!_coroutineDiction.ContainsKey(ownerInstanceId))
        {
            _coroutineDiction[ownerInstanceId] = new Dictionary<string, Coroutine>();
        }
        
        if (_coroutineDiction[ownerInstanceId].ContainsKey(routineName) && _coroutineDiction[ownerInstanceId][routineName] is not null)
        {
            StopSafeCoroutine(ownerInstanceId, routine);
        }
        
        return base.StartCoroutine(CoroutinePlayRoutine(ownerInstanceId, routine));
    }

    public void StopSafeCoroutine(int ownerInstanceId, IEnumerator routine)
    {
        var routineName = routine.ToString();
        base.StopCoroutine(_coroutineDiction[ownerInstanceId][routineName]);
        _coroutineDiction[ownerInstanceId][routineName] = null;
    }

    private IEnumerator CoroutinePlayRoutine(int ownerInstanceId, IEnumerator routine)
    {
        var routineName = routine.ToString();
        _coroutineDiction[ownerInstanceId][routineName] = base.StartCoroutine(routine);
        yield return _coroutineDiction[ownerInstanceId][routineName];
        StopSafeCoroutine(ownerInstanceId, routine);
    }
}