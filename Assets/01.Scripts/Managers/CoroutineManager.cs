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

    public void StartSafeCoroutine(int ownerInstanceId, IEnumerator routine)
    {
        var routineName = routine.ToString();

        var alreadyIncludeInstance = _coroutineDiction.ContainsKey(ownerInstanceId);
        
        if (!alreadyIncludeInstance)
        {
            _coroutineDiction[ownerInstanceId] = new Dictionary<string, Coroutine>();
        }
        
        var includeRoutine = _coroutineDiction[ownerInstanceId].ContainsKey(routineName);
        var isPlayRoutine = _coroutineDiction[ownerInstanceId][routineName] is not null;
        
        if (includeRoutine && isPlayRoutine)
        {
            StopSafeCoroutine(ownerInstanceId, routine);
        }
        
        base.StartCoroutine(CoroutinePlayRoutine(ownerInstanceId, routine));
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