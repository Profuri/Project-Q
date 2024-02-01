using System;
using System.Collections;
using System.Collections.Generic;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class LaserToggleObject : InteractableObject
{
    [SerializeField] private List<InteractableObject> _affectedObjects;

    [SerializeField] private float _affectedDelay;

    private Light _pointLight;

    private const float ToggleCancelDelay = 0.1f;
    private bool _isToggle;
    private float _lastToggleTime;

    private int _interactableIndex;

    private Coroutine _runningRoutine;

    public void Awake()
    {
        _pointLight = transform.Find("Point Light").GetComponent<Light>();
        _runningRoutine = null;
        Toggled(false);
    }

    private void Update()
    {
        if(_isToggle)
        {
            for (var i = 0; i < _affectedObjects.Count && i <= _interactableIndex; i++)
            {
                _affectedObjects[i].OnInteraction(null, _isToggle);
            }
            
            if (_lastToggleTime + ToggleCancelDelay <= Time.time)
            {
                Toggled(false);
            }
        }
        else
        {
            if (_affectedObjects.Count != 0)
            {
                _affectedObjects[_interactableIndex].OnInteraction(null, _isToggle);
            }
        }
    }

    private void Toggled(bool value)
    {
        _isToggle = value;
        _pointLight.enabled = value;

        if (_runningRoutine is not null)
        {
            StopCoroutine(_runningRoutine);
        }
        _runningRoutine = StartCoroutine(InteractRoutine(value));
    }

    private IEnumerator InteractRoutine(bool value)
    {
        _interactableIndex = value ? 0 : _affectedObjects.Count - 1;
        var dest = value ? _affectedObjects.Count - 1 : 0;
        while (Mathf.Abs(_interactableIndex - dest) != 0)
        {
            yield return new WaitForSeconds(_affectedDelay);
            _interactableIndex += value ? 1 : -1;
        }
    }

    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        _lastToggleTime = Time.time;
        if (!_isToggle)
        {
            Toggled(true);
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_affectedObjects.Count == 0)
        {
            return;
        }
        
        Gizmos.color = Color.black;
        foreach (var obj in _affectedObjects)
        {
            Gizmos.DrawLine(transform.position, obj.transform.position);
        }
    }
#endif
}
