using System.Collections.Generic;
using UnityEngine;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine.Events;

public class TogglePlate : ToggleTypeInteractableObject
{
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;
    
    private Transform _pressureMainTrm;

    private SoundEffectPlayer _soundEffectPlayer;
    
    public override void Awake()
    {
        base.Awake();
        _pressureMainTrm = transform.Find("PressureMain");
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _isToggle = false;
        LastToggleState = false;

        _soundEffectPlayer = new SoundEffectPlayer(this);
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        _isToggle = !_isToggle;
        LastToggleState = _isToggle;
        SoundManager.Instance.PlaySFX("Toggle",false, _soundEffectPlayer);
        CallToggleChangeEvents(_isToggle);
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        ToggleUpdate();
    }
    
    private void ToggleUpdate()
    {
        InteractAffectedObjects(_isToggle);
        var scale = _pressureMainTrm.localScale;
        scale.y = _isToggle ? _minHeight : _maxHeight;
        _pressureMainTrm.localScale = scale;
    }
}

