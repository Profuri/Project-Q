using System.Collections;
using System.Collections.Generic;
using ManagingSystem;
using UnityEngine;

public class CoroutineManager : BaseManager<CoroutineManager>
{
    private Dictionary<int, Coroutine> _coroutineDiction;

    public override void Init()
    {
        base.Init();
        _coroutineDiction = new Dictionary<int, Coroutine>();
    }

    public override void StartManager()
    {
        _coroutineDiction.Clear();
    }

    public void PlayCoroutine(IEnumerator routine)
    {
        var hash = routine.GetHashCode();
        
        if (_coroutineDiction.ContainsKey(hash) && _coroutineDiction[hash] is not null)
        {
            StopCoroutine(_coroutineDiction[hash]);
            _coroutineDiction[hash] = null;
        }
        
        StartCoroutine(CoroutinePlayRoutine(hash, routine));
    }

    private IEnumerator CoroutinePlayRoutine(int hash, IEnumerator routine)
    {
        yield return StartCoroutine(routine);
        _coroutineDiction[hash] = null;
    }
}