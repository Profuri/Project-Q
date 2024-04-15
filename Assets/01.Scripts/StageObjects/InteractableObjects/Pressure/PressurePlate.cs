using System.Collections.Generic;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class PressurePlate : InteractableObject
{
    [SerializeField] private LayerMask _pressionorMask;

    [SerializeField] private float _pressSpeed;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;


    [SerializeField] private List<AffectedObject> _affectedObjects;
    [SerializeField] private List<ToggleChangeEvent> _onToggleChangeEvents;

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

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _lastToggleState = false;
        foreach (var toggleChangeEvent in _onToggleChangeEvents)
        {
            toggleChangeEvent.Invoke(_lastToggleState);
        }
    }

    public override void UpdateUnit()
    { 
        base.UpdateUnit();

        var curToggleState = CheckPressed();
        if (_lastToggleState != curToggleState)
        {
            foreach (var toggleChangeEvent in _onToggleChangeEvents)
            {
                toggleChangeEvent.Invoke(curToggleState);
            }
        }
        _lastToggleState = curToggleState;
        OnInteraction(null, _lastToggleState);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        base.OnInteraction(communicator, interactValue, param);

        foreach (var obj in _affectedObjects)
        {
            obj?.Invoke(null, interactValue);
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
        var checkPos = Collider.bounds.center;
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
            var checkPos = Collider.bounds.center;
            var checkSize = _pressureObjTrm.localScale;
            Gizmos.DrawWireCube(checkPos, checkSize);
        }

        if (_affectedObjects.Count > 0)
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
