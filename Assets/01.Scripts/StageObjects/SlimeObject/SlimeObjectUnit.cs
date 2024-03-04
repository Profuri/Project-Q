using System;
using System.Collections.Generic;
using AxisConvertSystem;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class CheckCollision
{
    public Vector3 checkCenterPos = Vector3.zero;
    public Vector3 checkScale = Vector3.zero;
}

public class SlimeObjectUnit : ObjectUnit
{
    [Header("SlimeSettings")]
    [SerializeField] private float _bouncePower = 5f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private AxisType _applyAxisType;
    [SerializeField] private float _bounceTime = 0.5f;

    [SerializeField] private float _slimeMass = 0.1f;

    [SerializeField] private List<CheckCollision> _collisionList = new List<CheckCollision>();


    //x z 콜라이더 꺼
    private bool _canImpact = false;
    //�̰� ���߿� �������̽��� �ٲ�ߵɰ� ����.
    private List<Tuple<ObjectUnit,Vector3>> _movementUnitList;
    private bool _canFindModule;
    private AxisType _prevAxisType = AxisType.None;
    
    public override void Awake()
    {
        base.Awake();
        _canFindModule = true;
        _movementUnitList = new();
    }

    public override void Convert(AxisType axisType)
    {
        Debug.Log($"AxisType: {axisType}");
        if (axisType == AxisType.None)
        {
            _canImpact = (_applyAxisType & _prevAxisType) != 0;
            _canFindModule = true;
            Collider.enabled = true;
        }
        else
        {
            if (axisType != AxisType.Y)
            {
                Collider.enabled = false;
            }
            _prevAxisType = axisType;
            _canFindModule = false;
        }


        if (_canImpact)
        {
            //�̰� �÷��̾��� ��ġ�� �Ű����� ����Ǿ���ϴ� �κ�
            ShowBounceEffect(axisType,SlimeImpact);
        }

        base.Convert(axisType);
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        if (_canFindModule)
        {
            FindModule();
        }
    }

    private void FindModule()
    {
        _movementUnitList.Clear();
        var moduleList = GetMovementUnit();

        foreach (var movementModule in moduleList)
        {
            if (movementModule != null)
            {
                Vector3 bounceDirection = GetBounceDirection(movementModule.transform.position);

                var tuple = Tuple.Create(movementModule, bounceDirection);
                _movementUnitList.Add(tuple);
            }
        }
    }


    //이건 통통 튀기는거.
    private void SlimeEffect(Tuple<ObjectUnit,Vector3> tuple)
    {
        Debug.Log($"SlimeEffect, Module: {tuple.Item1}");
        var movementModule = tuple.Item1;
        Vector3 bounceDirection = tuple.Item2;

        Vector3 velocity = movementModule.Rigidbody.velocity;
        float bouncePower = velocity.magnitude * _slimeMass;

        if (bouncePower > 0.1f)
        {
            movementModule.SetVelocity(bounceDirection * bouncePower, false);
        }
    }

    //이건 압축 해제되었을 때 팍 튕겨나가는거.
    private void SlimeImpact()
    {
        foreach(var movementModule in _movementUnitList)
        {
            if (movementModule != null)
            {
                //_movementModule.CanJump = true;
                Vector3 bounceDirection = movementModule.Item2;
                ObjectUnit unit = movementModule.Item1;

                Debug.Log($"BounceDirection: {bounceDirection}");
                unit.SetVelocity(bounceDirection * _bouncePower, false);
                Debug.Log($"Velocity: {unit.Rigidbody.velocity}");
            }
        }

        _canImpact = false;
    }

    private void ShowBounceEffect(AxisType axisType, Action Callback)
    {
        Vector3 originScale = transform.localScale;
        Vector3 targetScale = transform.localScale;
        if (axisType == AxisType.None) return;
        else if (axisType == AxisType.Y) targetScale = new Vector3(1, 0, 1);

        transform.localScale = targetScale;


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

    private List<ObjectUnit> GetMovementUnit()
    {
        List<ObjectUnit> unitList = new List<ObjectUnit>();

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

                    if (col.TryGetComponent(out ObjectUnit unit))
                    {
                        if (!unit.staticUnit)
                        {
                            Debug.Log($"FindModule: {unit}");
                            unitList.Add(unit);
                        }
                    }
                }
            }
        }
        
        return unitList;
    }



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        var col = GetComponent<Collider>();
        foreach(CheckCollision collision in _collisionList)
        {

            Vector3 checkCenterPos = collision.checkCenterPos;
            Vector3 checkScale = collision.checkScale;
            //center ��ġ�� �����ؾߵ�
            if (checkCenterPos == Vector3.zero) checkCenterPos = transform.position;
            if (checkScale == Vector3.zero) checkScale = col.bounds.size;

            Gizmos.DrawCube(checkCenterPos, checkScale);
        }


    }
#endif
}
