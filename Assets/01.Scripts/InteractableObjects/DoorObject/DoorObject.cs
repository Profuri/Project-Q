using System.Collections;
using System.Collections.Generic;
using InteractableSystem;
using StageStructureConvertSystem;
using UnityEngine;

public class DoorObject : InteractableObject
{
    [Tooltip("IsOn이 켜져있는 경우 문이 열려있는 상태")]
    [SerializeField] private bool _isOn;
    [SerializeField] private Vector3 _targetPos;
    [SerializeField] private float _moveTime = 5f;

    private Vector3 _originPos;

    private void Awake()
    {
        _originPos = transform.position;
    }

    private void Update()
    {
        float diff = Vector3.Distance(transform.position, _targetPos);

        if (_isOn)
        {
            // isOn이 참이면 _targetPos로 이동
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * _moveTime);
        }
        else
        {
            // isOn이 거짓이면 _originPos로 이동
            transform.position = Vector3.Lerp(transform.position, _originPos, Time.deltaTime * _moveTime);
        }
    }


    public override void OnInteraction(StructureObjectUnitBase communicator, bool interactValue, params object[] param)
    {
        _isOn = interactValue;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (_targetPos == Vector3.zero) _targetPos = transform.position;

        Gizmos.DrawWireCube(_targetPos,transform.localScale);
    }
#endif
}
