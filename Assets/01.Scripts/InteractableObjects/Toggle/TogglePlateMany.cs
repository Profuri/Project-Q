using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractableSystem;
using System;

public class TogglePlateMany : InteractableObject
{
    [SerializeField] private LayerMask _pressionorMask;

    [SerializeField] private float _pressSpeed;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;

    [SerializeField] private List<InteractableObject> _affectedObjects;

    private Transform _pressureMainTrm;
    private Transform _pressureObjTrm;

    private bool _isToggle = false;
    private bool _input = false;

    private void Awake()
    {
        _pressureMainTrm = transform.Find("PressureMain");
        _pressureObjTrm = _pressureMainTrm.Find("PressureObject");
    }

    private void Update()
    {
        ToggleUpdate();
    }

    private void ToggleUpdate()
    {
        for (int i = 0; i < _affectedObjects.Count; i++)
        {
            _affectedObjects[i]?.OnInteraction(null, _isToggle);
        }

        var scale = _pressureMainTrm.localScale;
        scale.y = _isToggle ? _minHeight : _maxHeight;
        _pressureMainTrm.localScale = scale;
    }

    public override void OnInteraction(PlayerController player, bool interactValue)
    {
        _isToggle = !_isToggle;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_pressureObjTrm)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        var checkPos = _pressureObjTrm.position
            + Vector3.up
            * (_pressureObjTrm.localScale.y * _pressureMainTrm.localScale.y / 2 + _pressureObjTrm.localScale.y / 2);
        var checkSize = _pressureObjTrm.localScale;
        Gizmos.DrawWireCube(checkPos, checkSize);
    }
#endif
}

