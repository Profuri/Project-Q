using System.Collections;
using System.Collections.Generic;
using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class LaserToggleObject : InteractableObject
{
    [SerializeField] private List<InteractableObject> _affectedObject;

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
            for (var i = 0; i <= _interactableIndex; i++)
            {
                _affectedObject[i].OnInteraction(null, _isToggle);
            }
            
            if (_lastToggleTime + ToggleCancelDelay <= Time.time)
            {
                Toggled(false);
            }
        }
        else
        {
            _affectedObject[_interactableIndex].OnInteraction(null, _isToggle);
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
        _interactableIndex = value ? 0 : _affectedObject.Count - 1;
        var dest = value ? _affectedObject.Count - 1 : 0;
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
}
