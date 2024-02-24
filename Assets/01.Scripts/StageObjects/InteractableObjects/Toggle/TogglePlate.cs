using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractableSystem;
using System;
using AxisConvertSystem;

public class TogglePlate : InteractableObject
{
    [SerializeField] private LayerMask _pressionorMask;

    [SerializeField] private float _pressSpeed;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;
    [SerializeField] private List<InteractableObject> _affectedObjects;

    [Header("Delay")]
    [SerializeField] private bool _useDelay = false;
    [SerializeField] private bool _rollbackReverse = false;
    [SerializeField] private float _delayTime = 0.5f;

    private List<InteractableObject> _activeObjects;
    private Transform _pressureMainTrm;
    private Transform _pressureObjTrm;

    private bool _isToggle = false;
    private bool _isPlaying = false;

    public override void Awake()
    {
        base.Awake();
        _pressureMainTrm = transform.Find("PressureMain");
        _pressureObjTrm = _pressureMainTrm.Find("PressureObject");
        _isToggle = false;
        _isPlaying = false;
        if (!_useDelay)
            _activeObjects = _affectedObjects;
        else
            _activeObjects = new();
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if(_isPlaying == true)
        {
            Debug.Log("Toggle�� �� �����ϴ�.");
        }
        else
        {
            _isToggle = !_isToggle;
            if(_useDelay)
                StartCoroutine(ToggleObjects(_isToggle));
        }
    }

    public override void Update()
    {
        base.Update();
        ToggleUpdate();
    }

    private void ToggleUpdate()
    {
        if (_activeObjects == null) return;

        for (int i = 0; i < _activeObjects.Count; i++)
        {
            _activeObjects[i]?.OnInteraction(null, _isToggle);
        }
        var scale = _pressureMainTrm.localScale;
        scale.y = _isToggle ? _minHeight : _maxHeight;
        _pressureMainTrm.localScale = scale;
    }


    private IEnumerator ToggleObjects(bool _isToggle)
    {
        _isPlaying = true;
        for (int i = 0; i < _affectedObjects.Count; i++)
        {
            _activeObjects.Add(_affectedObjects[i]);
            _activeObjects[i]?.OnInteraction(null, _isToggle);
            yield return new WaitForSeconds(_delayTime);
        }
            
        for(int i = 0; i < _activeObjects.Count; i++)
            yield return new WaitUntil(() => _activeObjects[i].InterEnd);
        

        if(_rollbackReverse) _affectedObjects.Reverse();
        _activeObjects.ForEach((obj) => obj.InterEnd = false);
        _activeObjects.Clear();
        _isPlaying = false;
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

        if (_affectedObjects.Count != 0)
        {
            Gizmos.color = Color.black;
            foreach (var obj in _affectedObjects)
            {
                if (obj == null)
                {
                    return;
                }
                Gizmos.DrawLine(transform.position, obj.transform.position);
            }
        }
    }
#endif
}

