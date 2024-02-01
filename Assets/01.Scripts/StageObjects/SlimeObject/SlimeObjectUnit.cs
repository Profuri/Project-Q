using System;
using StageStructureConvertSystem;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
[RequireComponent(typeof(Collider),typeof(Rigidbody))]


[System.Serializable]
public class CheckCollision
{
    public Vector3 checkCenterPos = Vector3.zero;
    public Vector3 checkScale = Vector3.zero;
}

public class SlimeObjectUnit : StructureObjectUnitBase
{
    [Header("SlimeSettings")]
    [SerializeField] private float _bouncePower = 5f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private EAxisType _applyAxisType;
    [SerializeField] private float _bounceTime = 0.5f;

    [SerializeField] private float _slimeMass = 0.1f;

    [SerializeField] private List<CheckCollision> _collisionList = new List<CheckCollision>();


    //x z 콜라이더 꺼
    private bool _canImpact = false;
    //�̰� ���߿� �������̽��� �ٲ�ߵɰ� ����.
    private List<Tuple<PlayerMovementModule,Vector3>> _movementModuleList;
    private bool _canFindModule;
    private EAxisType _prevAxisType = EAxisType.NONE;

    public override void Init(StructureConverter converter)
    {
        _canFindModule = true;
        _movementModuleList = new();
        base.Init(converter);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObj = collision.collider.gameObject;

        if (collisionObj.CompareTag("Player"))
        {
            var controller = collisionObj.GetComponent<PlayerController>();
            var movementModule = controller.GetModule<PlayerMovementModule>();
            Vector3 bounceDirection = GetBounceDirection(movementModule.transform.position);
            var tuple = Tuple.Create(movementModule, bounceDirection);
            SlimeEffect(tuple);
        }
    }
    
    public override void TransformSynchronization(EAxisType axisType)
    {
        Debug.Log($"AxisType: {axisType}");
        if (axisType == EAxisType.NONE)
        {
            _canImpact = (_applyAxisType & _prevAxisType) != 0;
            _canFindModule = true;
            _collider.enabled = true;
        }
        else
        {
            if (axisType != EAxisType.Y)
            {
                _collider.enabled = false;
            }
            _canFindModule = false;
        }

        _prevAxisType = axisType;

        if (_canImpact)
        {
            //�̰� �÷��̾��� ��ġ�� �Ű����� ����Ǿ���ϴ� �κ�
            ShowBounceEffect(SlimeImpact);
        }

        base.TransformSynchronization(axisType);


    }

    private void Update()
    {
        if (_canFindModule)
        {
            FindModule();
        }
    }

    private void FindModule()
    {
        _movementModuleList.Clear();
        var moduleList = GetPlayerMovementModule();

        foreach (var movementModule in moduleList)
        {
            if (movementModule != null)
            {
                Vector3 bounceDirection = GetBounceDirection(movementModule.transform.position);

                var tuple = Tuple.Create(movementModule, bounceDirection);
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
                Debug.Log($"Player Velocity: {playerMovementMoudle.MoveVelocity}");
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

    private Vector3 GetBounceDirection(Vector3 modulePos)
    {
        Vector3 diffPos = modulePos - transform.position;
        Vector3 returnDirection = Vector3.up;

        if(diffPos.x > diffPos.y)
        {
            if(diffPos.z > diffPos.x)
            {
                returnDirection = Vector3.back;
            }
            else
            {
                returnDirection = Vector3.right;
            }
        }
        return returnDirection;
    }

    private List<PlayerMovementModule> GetPlayerMovementModule()
    {
        List<PlayerMovementModule> movementModuleList = new List<PlayerMovementModule>();

        foreach (CheckCollision collision in _collisionList)
        {
            Vector3 checkScale = collision.checkScale;
            Vector3 checkCenterPos = collision.checkCenterPos;


            Vector3 halfExtents = checkScale * 0.5f;
            Quaternion rotation = transform.rotation;

            Collider[] cols = Physics.OverlapBox(checkCenterPos, halfExtents, rotation, _targetLayer);

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
                        Debug.Log($"FindModule: {movementModule}");
                        movementModuleList.Add(movementModule);
                    }
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


        foreach(CheckCollision collision in _collisionList)
        {

            Vector3 checkCenterPos = collision.checkCenterPos;
            Vector3 checkScale = collision.checkScale;
            //center ��ġ�� �����ؾߵ�
            if (checkCenterPos == Vector3.zero) checkCenterPos = transform.position;
            if (checkScale == Vector3.zero) checkScale = _collider.bounds.size;

            Gizmos.DrawCube(checkCenterPos, checkScale);
        }


    }
#endif
}
