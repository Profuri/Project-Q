using System.Collections;
using System.Collections.Generic;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class LaserToggleObject : ToggleTypeInteractableObject
{
    private Light _pointLight;

    private const float ToggleCancelDelay = 0.1f;
    private float _lastToggleTime;

    private int _interactableIndex;

    public override void Awake()
    {
        base.Awake();
        
        _pointLight = transform.Find("Point Light").GetComponent<Light>();
        Toggled(false);
        CallToggleChangeEvents(_isToggle);
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        InteractAffectedObjects(_isToggle);

        if(_isToggle)
        {
            if (_lastToggleTime + ToggleCancelDelay <= Time.time)
            {
                Toggled(false);
                CallToggleChangeEvents(_isToggle);
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
            CallToggleChangeEvents(_isToggle);
        }
    }
}
