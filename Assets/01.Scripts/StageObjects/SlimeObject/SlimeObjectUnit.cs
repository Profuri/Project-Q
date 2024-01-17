using StageStructureConvertSystem;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
[RequireComponent(typeof(Collider))]

public class SlimeObjectUnit : StructureObjectUnitBase
{
    [Header("SlimeSettings")]
    [SerializeField] private float _bouncePower = 5f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private EAxisType _applyAxisType;


    [Header("SlimeCollisionSettings")]
    [Tooltip("If you setting this Vector3.zero, it will be set default settings")]
    [SerializeField] private Vector3 _checkCenterPos = Vector3.zero;
    [Tooltip("If you setting this Vector3.zero, it will be set default settings")]
    [SerializeField] private Vector3 _checkScale = Vector3.zero;

    private bool _canImpact = false;
    //이거 나중에 인터페이스로 바꿔야될거 같음.
    private PlayerMovementModule _movementModule;
    private Vector3 _bounceDirection;
    private bool _canFindModule;
    private EAxisType _prevAxisType = EAxisType.NONE;

    public override void Init(StructureConverter converter)
    {
        _canFindModule = true;
        base.Init(converter);
    }

    public override void TransformSynchronization(EAxisType axisType)
    {
        base.TransformSynchronization(axisType);



        if (axisType == EAxisType.NONE)
        {
            _canImpact = (_applyAxisType & _prevAxisType) != 0;
            _canFindModule = true;
        }
        else
        {
            _canFindModule = false;
        }

        _prevAxisType = axisType;

        if (_canImpact)
        {
            //이게 플레이어의 위치가 옮겨지고 실행되어야하는 부분
            SlimeImpact();
        }
    }

    private void Update()
    {
        Debug.Log(_movementModule);
        if (_canFindModule)
        {
            _movementModule = GetPlayerMovementModule();
            if (_movementModule != null)
            {
                _bounceDirection = _movementModule.transform.position - transform.position;
            }
            else
            {
                _bounceDirection = Vector3.zero;
            }
        }
    }

    private void SlimeImpact()
    {
        if (_movementModule != null)
        {
            //_movementModule.CanJump = true;


            Debug.Log($"BounceDirection: {_bounceDirection}");
            _movementModule.SetForce(_bounceDirection * _bouncePower);
        }
        _canImpact = false;
    }

    private PlayerMovementModule GetPlayerMovementModule()
    {
        PlayerMovementModule movementModule = null;
        Vector3 halfExtents = _checkScale * 0.5f;
        Quaternion rotation = transform.rotation;

        Collider[] cols = Physics.OverlapBox(_checkCenterPos, halfExtents, rotation, _targetLayer);

        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                //if (col.TryGetComponent(out IBounceable bounceable))
                //{
                //    bounceable.BounceOff();
                //}

                if (col.TryGetComponent(out PlayerController playerController))
                {
                    movementModule = playerController.GetModule<PlayerMovementModule>();
                }
            }
        }
        return movementModule;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (_collider == null)
        {
            _collider = GetComponent<Collider>();
        }

        //center 위치를 수정해야됨
        if (_checkCenterPos == Vector3.zero) _checkCenterPos = transform.position;
        if (_checkScale == Vector3.zero) _checkScale = _collider.bounds.size;

        Gizmos.DrawCube(_checkCenterPos,_checkScale);
    }
}
