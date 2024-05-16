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
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        Toggled(false);
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        InteractAffectedObjects(_isToggle);

        if(_isToggle && Converter.Convertable)
        {
            if (_lastToggleTime + ToggleCancelDelay <= Time.time)
            {
                Toggled(false);
            }
        }
    }

    private void Toggled(bool value)
    {
        _isToggle = value;
        LastToggleState = value;
        _pointLight.enabled = value;
        CallToggleChangeEvents(value);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        _lastToggleTime = Time.time;
        if (!_isToggle)
        {
            Toggled(true);
        }
    }
}
