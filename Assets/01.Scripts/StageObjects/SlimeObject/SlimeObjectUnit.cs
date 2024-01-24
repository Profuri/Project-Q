using System;
using StageStructureConvertSystem;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
[RequireComponent(typeof(Collider),typeof(Rigidbody))]

public class SlimeObjectUnit : StructureObjectUnitBase
{
    [Header("SlimeSettings")]
    [SerializeField] private float _bouncePower = 5f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private EAxisType _applyAxisType;
    [SerializeField] private float _bounceTime = 0.5f;
    [SerializeField] private Vector3 _bounceDirection;

    [SerializeField] private float _slimeMass = 0.1f;

    [Header("SlimeCollisionSettings")]
    [Tooltip("If you setting this Vector3.zero, it will be set default settings")]
    [SerializeField] private Vector3 _checkCenterPos = Vector3.zero;
    [Tooltip("If you setting this Vector3.zero, it will be set default settings")]
    [SerializeField] private Vector3 _checkScale = Vector3.zero;


    private bool _canImpact = false;
    //�̰� ���߿� �������̽��� �ٲ�ߵɰ� ����.
    private List<Tuple<PlayerMovementModule,Vector3>> _movementModuleList;
    private bool _canFindModule;
    private EAxisType _prevAxisType = EAxisType.NONE;

    public override void Init(StructureConverter converter)
    {
        _canFindModule = true;
        _movementModuleList = new ();
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
            //�̰� �÷��̾��� ��ġ�� �Ű����� ����Ǿ���ϴ� �κ�
            ShowBounceEffect(SlimeImpact);
        }

    }

    private void Update()
    {
        var collisionModuleList = GetCollisionModules();
        foreach (var collisionModule in collisionModuleList)
        {
            if (collisionModule != null)
            {
                Vector3 bounceDirection = collisionModule.transform.position - transform.position;
                SlimeEffect(Tuple.Create(collisionModule,bounceDirection.normalized));
            }
        }

        if (_canFindModule)
        {
            _movementModuleList.Clear();
            var moduleList = GetPlayerMovementModule();
            
            foreach(var movementModule in moduleList)
            {
                Vector3 bounceDirection;
                if (movementModule != null)
                {
                    bounceDirection = movementModule.transform.position - transform.position;
                }
                else
                {
                    bounceDirection = Vector3.zero;
                }
                
                var tuple = Tuple.Create(movementModule,bounceDirection);
                _movementModuleList.Add(tuple);
            }
        }
    }


    //이건 통통 튀기는거.
    private void SlimeEffect(Tuple<PlayerMovementModule,Vector3> tuple)
    {
        Debug.Log($"SlimeEffect, Module: {tuple.Item1}");
        var movementModule = tuple.Item1;
        Vector3 bounceDirection = tuple.Item2;

        Vector3 velocity = movementModule.MoveVelocity;
        float bouncePower = velocity.magnitude * _slimeMass;

        if (bouncePower > 0.1f)
        {
            movementModule.AddForce(bounceDirection * bouncePower);
        }
    }
    //이건 압축 해제되었을 때 팍 튕겨나가는거.
    private void SlimeImpact()
    {
        foreach(var movementModule in _movementModuleList)
        {
            if (movementModule != null)
            {
                //_movementModule.CanJump = true;
                Vector3 bounceDirection = movementModule.Item2;
                PlayerMovementModule playerMovementMoudle = movementModule.Item1;

                Debug.Log($"BounceDirection: {bounceDirection}");
                playerMovementMoudle.SetForce(bounceDirection * _bouncePower);
            }
        }

        _canImpact = false;
    }

    private void ShowBounceEffect(Action Callback)
    {
        Vector3 originScale = transform.localScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originScale * 0.8f,_bounceTime * 0.5f)).SetEase(Ease.InBounce);
        seq.Append(transform.DOScale(originScale * 1.2f,_bounceTime * 0.5f)).SetEase(Ease.InBounce);
        seq.Append(transform.DOScale(originScale, _bounceTime * 0.5f)).SetEase(Ease.InBounce);
        seq.AppendCallback(() => Callback?.Invoke());
    }
    private List<PlayerMovementModule> GetPlayerMovementModule()
    {
        Vector3 halfExtents = _checkScale * 0.5f;
        Quaternion rotation = transform.rotation;

        Collider[] cols = Physics.OverlapBox(_checkCenterPos, halfExtents, rotation, _targetLayer);
        List<PlayerMovementModule> movementModuleList = new List<PlayerMovementModule>();

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
                    var movementModule = playerController.GetModule<PlayerMovementModule>();
                    movementModuleList.Add(movementModule);
                }
            }
        }
        return movementModuleList;
    }

    private List<PlayerMovementModule> GetCollisionModules()
    {
        Vector3 halfExtents = _collider.bounds.extents * 0.55f;
        Quaternion rotation = transform.rotation;

        List<PlayerMovementModule> moduleList = new List<PlayerMovementModule>();
        Collider[] cols = Physics.OverlapBox(_checkCenterPos, halfExtents, rotation, _targetLayer);

        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                if (col.TryGetComponent(out PlayerController playerController))
                {
                    var movementModule = playerController.GetModule<PlayerMovementModule>();
                    moduleList.Add(movementModule);
                }
            }
        }

        return moduleList;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (_collider == null)
        {
            _collider = GetComponent<Collider>();
        }

        //center ��ġ�� �����ؾߵ�
        if (_checkCenterPos == Vector3.zero) _checkCenterPos = transform.position;
        if (_checkScale == Vector3.zero) _checkScale = _collider.bounds.size;

        Gizmos.DrawCube(_checkCenterPos,_checkScale);
    }
#endif
}
