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

    private EAxisType _prevAxisType = EAxisType.NONE;

    public override void Init(StructureConverter converter)
    {
        base.Init(converter);
    }

    public override void ConvertDimension(EAxisType axisType)
    {
        bool canImpact = false;                         


        if (axisType == EAxisType.NONE)
        {
            canImpact = (_applyAxisType & _prevAxisType) != 0;
        }

        _prevAxisType = axisType;


        Debug.Log($"CanImpact: {canImpact}");


        base.ConvertDimension(axisType);

        if (canImpact)
        {
            SlimeImpact();
        }
    }

    private void SlimeImpact()
    {
        Debug.Log($"PlayerColliderPos: {GameManager.Instance.PlayerController.CharController.transform.position}");
        Debug.Break();

        Vector3 halfExtents = _checkScale * 0.5f;
        Quaternion rotation = transform.rotation;
        Collider[] cols = Physics.OverlapBox(_checkCenterPos,halfExtents,rotation,_targetLayer);

        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                Debug.Log($"ObjName: {col.gameObject.name}");
                //if (col.TryGetComponent(out IBounceable bounceable))
                //{
                //    bounceable.BounceOff();
                //}

                if (col.TryGetComponent(out PlayerController playerController))
                {
                    Debug.Log("PlayerIsNow");
                    var movementModule = playerController.GetModule<PlayerMovementModule>();

                    Vector3 bounceDirection = playerController.transform.position - transform.position;
                    movementModule.AddForce(bounceDirection * _bouncePower);
                }
            }
        }
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
