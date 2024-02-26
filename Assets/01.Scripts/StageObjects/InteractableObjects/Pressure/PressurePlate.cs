using System.Collections.Generic;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : InteractableObject
{
    [SerializeField] private LayerMask _pressionorMask;

    [SerializeField] private float _pressSpeed;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;

    [SerializeField] private List<InteractableObject> _affectedObjects;

    [SerializeField] private UnityEvent<bool> _onToggleChangeEvent;

    private Transform _pressureMainTrm;
    private Transform _pressureObjTrm;

    private bool _lastToggleState;

    public override void Awake()
    {
        base.Awake();
        _lastToggleState = false;
        _pressureMainTrm = transform.Find("PressureMain");
        _pressureObjTrm = _pressureMainTrm.Find("PressureObject");
    }

    public override void Update()
    {
        base.Update();
        if (_lastToggleState != CheckPressed())
        {
            _lastToggleState = !_lastToggleState;
            _onToggleChangeEvent?.Invoke(_lastToggleState);
        }
        OnInteraction(null, _lastToggleState);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        foreach (var obj in _affectedObjects)
        {
            obj?.OnInteraction(null, interactValue);
        }
        
        var current = _pressureMainTrm.localScale.y;
        var dest = interactValue ? _minHeight : _maxHeight;

        if (Mathf.Abs(dest - current) <= 0.01f)
        {
            current = dest;
        }
        
        var scale = _pressureMainTrm.localScale;
        scale.y = Mathf.Lerp(current, dest, Time.deltaTime * _pressSpeed);
        _pressureMainTrm.localScale = scale;

    }

    private bool CheckPressed()
    {
        var checkPos = _pressureObjTrm.position
            + Vector3.up 
            * (_pressureObjTrm.localScale.y * _pressureMainTrm.localScale.y / 2 + _pressureObjTrm.localScale.y / 2);
        var checkSize = _pressureObjTrm.localScale;

        var cols = new Collider[2];
        var size = Physics.OverlapBoxNonAlloc(checkPos, checkSize / 2, cols, Quaternion.identity, _pressionorMask);

        if (size <= 1)
        {
            return false;
        }

        if (cols[1].TryGetComponent<InteractableObject>(out var interactable))
        {
            return interactable.Attribute.HasFlag(EInteractableAttribute.CAN_PRESS_THE_PRESSURE_PLATE);
        }

        return true;
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

        if (_affectedObjects.Count > 0)
        {
            Gizmos.color = Color.black;
            foreach (var obj in _affectedObjects)
            {
                Gizmos.DrawLine(transform.position, obj.transform.position);
            }
        }
    }
#endif
}
