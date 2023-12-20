using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class LaserToggleObject : InteractableObject
{
    [SerializeField] private InteractableObject _affectedObject;

    private Light _pointLight;

    private const float ToggleCancelDelay = 0.1f;
    private bool _isToggle;
    private float _lastToggleTime;

    public void Awake()
    {
        _pointLight = transform.Find("Point Light").GetComponent<Light>();
        Toggled(false);
    }

    private void Update()
    {
        if (!_isToggle)
        {
            return;
        }

        if (_lastToggleTime + ToggleCancelDelay <= Time.time)
        {
            Toggled(false);
        }
    }

    private void Toggled(bool value)
    {
        _isToggle = value;
        _pointLight.enabled = value;

        if (value)
        {
            _lastToggleTime = Time.time;
        }
    }

    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        Toggled(true);
        
        if (_affectedObject is not null)
        {
            _affectedObject.OnInteraction(communicator, interactValue);
        }
    }
}
