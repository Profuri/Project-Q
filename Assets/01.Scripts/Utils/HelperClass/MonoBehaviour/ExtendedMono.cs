using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedMono : MonoBehaviour
{
    private readonly Dictionary<string, Coroutine> _runningRoutines = new Dictionary<string, Coroutine>();

    public Coroutine StartSafeCoroutine(string routineName, IEnumerator routine)
    {
        if (!gameObject.activeSelf)
        {
            Debug.LogWarning("You can't start safe coroutine active false object.");
            return null;
        }
            
        StopSafeCoroutine(routineName);
        _runningRoutines[routineName] = StartCoroutine(routine);
        return _runningRoutines[routineName];
    }

    public void StopSafeCoroutine(string routineName)
    {
        if (_runningRoutines.ContainsKey(routineName) && _runningRoutines[routineName] != null)
        {
            StopCoroutine(_runningRoutines[routineName]);
        }
    }
}