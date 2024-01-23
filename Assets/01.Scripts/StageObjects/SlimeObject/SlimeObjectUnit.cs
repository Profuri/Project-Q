using System;
using StageStructureConvertSystem;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
[RequireComponent(typeof(Collider))]

public class SlimeObjectUnit : StructureObjectUnitBase
{
    [Header("SlimeSettings")]
    [SerializeField] private float _bouncePower = 5f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private EAxisType _applyAxisType;
    [SerializeField] private float _bounceTime = 0.5f;

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