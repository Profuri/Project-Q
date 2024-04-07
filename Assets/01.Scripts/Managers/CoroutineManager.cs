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

    public void StartCoroutine(int owner, IEnumerator routine)
    {
        var routineName = routine.ToString();

        if (!_coroutineDiction.ContainsKey(owner))
        {
            _coroutineDiction[owner] = new Dictionary<string, Coroutine>();
        }
        
        if (_coroutineDiction[owner].ContainsKey(routineName) && _coroutineDiction[owner][routineName] is not null)
        {
            StopCoroutine(owner, routine);
        }
        
        base.StartCoroutine(CoroutinePlayRoutine(owner, routine));
    }

    public void StopCoroutine(int owner, IEnumerator routine)
    {
        var routineName = routine.ToString();
        base.StopCoroutine(_coroutineDiction[owner][routineName]);
        _coroutineDiction[owner][routineName] = null;
    }

    private IEnumerator CoroutinePlayRoutine(int owner, IEnumerator routine)
    {
        var routineName = routine.ToString();
        _coroutineDiction[owner][routineName] = base.StartCoroutine(routine);
        yield return _coroutineDiction[owner][routineName];
        StopCoroutine(owner, routine);
    }
}