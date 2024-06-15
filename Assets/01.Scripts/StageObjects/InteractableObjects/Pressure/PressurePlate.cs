using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class PressurePlate : ToggleTypeInteractableObject
{
    [SerializeField] private LayerMask _pressionorMask;

    [SerializeField] private float _pressSpeed;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;

    [SerializeField] private float _yScaleOffset = 1.2f;

    private Transform _pressureMainTrm;
    private Transform _pressureObjTrm;

    private SoundEffectPlayer _soundEffectPlayer;

    public override void Awake()
    {
        base.Awake();
        _pressureMainTrm = transform.Find("PressureMain");
        _pressureObjTrm = _pressureMainTrm.Find("PressureObject");

        _soundEffectPlayer = new SoundEffectPlayer(this);
    }

    public override void UpdateUnit()
    { 
        base.UpdateUnit();

        if (!Converter.Convertable)
        {
            return;
        }

        var curToggleState = CheckPressed();
        if (LastToggleState != curToggleState)
        {
            CallToggleChangeEvents(curToggleState);
            if(curToggleState)
            {
                SoundManager.Instance.PlaySFX("PressPanel",false,_soundEffectPlayer);
            }
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
        var checkSize = _pressureObjTrm.localScale / 2f;
        checkSize.y *= _yScaleOffset;

        var cols = new Collider[10];
        var size = Physics.OverlapBoxNonAlloc(checkPos, checkSize, cols, Quaternion.identity, _pressionorMask);

        var returnValue = false;
        
        for (var i = 0; i < size; i++)
        {
            if (cols[i] == Collider)
            {
                continue;
            }

            if (!cols[i].TryGetComponent<ObjectUnit>(out var unit))
            {
                continue;
            }

            if (unit is InteractableObject interactable)
            {
                if (interactable.Attribute.HasFlag(EInteractableAttribute.CAN_PRESS_THE_PRESSURE_PLATE))
                {
                    returnValue = true;
                    break;
                }
            }
            else
            {
                returnValue = true;
                break;
            }
        }

        return returnValue;
    }
}
