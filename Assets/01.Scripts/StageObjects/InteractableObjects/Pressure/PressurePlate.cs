using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;

public class PressurePlate : ToggleTypeInteractableObject, IUpdateBlockable
{
    [SerializeField] private LayerMask _pressionorMask;

    [SerializeField] private float _pressSpeed;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;
    [SerializeField] private float _yScaleOffset = 1.2f;
    private Transform _pressureMainTrm;
    private Transform _pressureObjTrm;


    public override void Awake()
    {
        base.Awake();
        _pressureMainTrm = transform.Find("PressureMain");
        _pressureObjTrm = _pressureMainTrm.Find("PressureObject");
    }

    public override void UpdateUnit()
    { 
        base.UpdateUnit();
        var curToggleState = CheckPressed();
        if (LastToggleState != curToggleState)
        {
            CallToggleChangeEvents(curToggleState);
        }
        LastToggleState = curToggleState;
        OnInteraction(null, LastToggleState);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        InteractAffectedObjects(interactValue);
        
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
        checkSize.y *= _yScaleOffset;

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
}
