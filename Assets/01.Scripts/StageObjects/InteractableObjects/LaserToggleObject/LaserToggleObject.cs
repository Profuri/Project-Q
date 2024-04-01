using System.Collections;
using System.Collections.Generic;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class LaserToggleObject : InteractableObject
{
    [SerializeField] private List<AffectedObject> _affectedObjects;
    [SerializeField] private List<ToggleChangeEvent> _onToggleChangeEvents;

    private Light _pointLight;

    private const float ToggleCancelDelay = 0.1f;
    private bool _isToggle;
    private float _lastToggleTime;

    private int _interactableIndex;

    public override void Awake()
    {
        base.Awake();
        
        _pointLight = transform.Find("Point Light").GetComponent<Light>();
        Toggled(false);
        AffectedToggleChange();
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        AffectedObject();
        
        if(_isToggle)
        {
            if (_lastToggleTime + ToggleCancelDelay <= Time.time)
            {
                Toggled(false);
                AffectedToggleChange();
            }
        }
    }

    private void Toggled(bool value)
    {
        _isToggle = value;
        _pointLight.enabled = value;
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        _lastToggleTime = Time.time;
        if (!_isToggle)
        {
            Toggled(true);
            AffectedToggleChange();
        }
    }

    private void AffectedObject()
    {
        foreach (var affectedObj in _affectedObjects)
        {
            affectedObj?.Invoke(this, _isToggle);
        }
    }

    private void AffectedToggleChange()
    {
        foreach (var toggleChangeEvent in _onToggleChangeEvents)
        {
            toggleChangeEvent?.Invoke(_isToggle);
        }
    }
    
#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (_affectedObjects.Count == 0)
        {
            return;
        }
        
        Gizmos.color = Color.black;
        foreach (var obj in _affectedObjects)
        {
            Gizmos.DrawLine(transform.position, obj.InteractableObject.transform.position);
        }
    }
#endif
}
