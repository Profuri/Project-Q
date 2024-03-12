using System.Collections.Generic;
using UnityEngine;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine.Events;

public class TogglePlate : InteractableObject
{
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;
    
    [SerializeField] private List<AffectedObject> _affectedObjects;
    [SerializeField] private List<ToggleChangeEvent> _onToggleChangeEvents;
    
    private Transform _pressureMainTrm;
    private Transform _pressureObjTrm;

    private bool _isToggle;

    public override void Awake()
    {
        base.Awake();
        _pressureMainTrm = transform.Find("PressureMain");
        _pressureObjTrm = _pressureMainTrm.Find("PressureObject");
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _isToggle = false;
        foreach (var toggleChangeEvent in _onToggleChangeEvents)
        {
            toggleChangeEvent.Invoke(_isToggle);
        }
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        _isToggle = !_isToggle;
        foreach (var toggleChangeEvent in _onToggleChangeEvents)
        {
            toggleChangeEvent.Invoke(_isToggle);
        }
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        ToggleUpdate();
    }

    private void ToggleUpdate()
    {
        for (int i = 0; i < _affectedObjects.Count; i++)
        {
            _affectedObjects[i]?.Invoke(null, _isToggle);
        }
        var scale = _pressureMainTrm.localScale;
        scale.y = _isToggle ? _minHeight : _maxHeight;
        _pressureMainTrm.localScale = scale;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_pressureObjTrm)
        {
            Gizmos.color = Color.yellow;
            var checkPos = _pressureObjTrm.position
                + Vector3.up
                * (_pressureObjTrm.localScale.y * _pressureMainTrm.localScale.y / 2 + _pressureObjTrm.localScale.y / 2);
            var checkSize = _pressureObjTrm.localScale;
            Gizmos.DrawWireCube(checkPos, checkSize);
        }

        if (_affectedObjects.Count != 0)
        {
            Gizmos.color = Color.black;
            foreach (var obj in _affectedObjects)
            {
                if (obj?.InteractableObject is null)
                {
                    continue;
                }
                Gizmos.DrawLine(transform.position, obj.InteractableObject.transform.position);
            }
        }
    }
#endif
}

