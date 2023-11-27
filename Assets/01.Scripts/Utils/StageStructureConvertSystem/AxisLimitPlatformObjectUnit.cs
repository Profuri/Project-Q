using StageStructureConvertSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisLimitPlatformObjectUnit : StructureObjectUnitBase
{
    //dddddddddddd

    [SerializeField] private EAxisType _targetAxisType;
    [SerializeField] private GameObject _priorityObject;
    [SerializeField] private float _colCheckDistance;
    [SerializeField] private LayerMask _targetLayer;

    private Collider _collider;
    private Coroutine _coroutine;

    private Collider _priorityCollider;


    public override void Init(StructureConverter converter)
    {
        base.Init(converter);

        _collider = GetComponent<Collider>();
        _priorityCollider = _priorityObject.GetComponent<Collider>();

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(CheckColCor());
    }

    private IEnumerator CheckColCor()
    {
        while (true)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, _colCheckDistance, _targetLayer);
            //if Player is checked
            if(cols.Length > 0)
            {
                _priorityCollider.isTrigger = true;
            }
            else
            {
                _priorityCollider.isTrigger = false;
            }
            yield return null;
        }
    }

    public override void ConvertDimension(EAxisType axisType)
    {
        _objectInfo.position = transform.localPosition;
        _objectInfo.scale = transform.localScale;

        // convert to 3D
        if (axisType == EAxisType.NONE)
        {
            (_prevObjectInfo, _objectInfo) = (_objectInfo, _prevObjectInfo);
            this.gameObject.SetActive(true);
        }
        // convert to 2D
        else
        {
            _prevObjectInfo = _objectInfo;
            _objectInfo.axis = axisType;

            if (axisType == _targetAxisType)
            {
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    public override void TransformSynchronization(EAxisType axisType)
    {
        if (_targetAxisType == axisType)
        {
            Vector3 priorityPos = _priorityObject.transform.position;
            //Vector3 priObjColBoundsDivTwo = _priorityObject.transform.GetComponent<Collider>().bounds.size;
            Vector3 priObjColBoundsDivTwo = Vector3.zero;

            int xDir = transform.position.x - priorityPos.x > 0 ? 1 : -1;
            int yDir = transform.position.y - priorityPos.y > 0 ? 1 : -1;
            int zDir = transform.position.z - priorityPos.z > 0 ? 1 : -1;

            switch (axisType)
            { 
                case EAxisType.NONE:
                    switch (_prevObjectInfo.axis)
                    {
                        // x compress
                        case EAxisType.X:
                            _objectInfo.position.y = _prevObjectInfo.position.y;
                            _objectInfo.position.z = _prevObjectInfo.position.z;
                            break;
                        // y compress
                        case EAxisType.Y:
                            _objectInfo.position.x = _prevObjectInfo.position.x;
                            _objectInfo.position.z = _prevObjectInfo.position.z;
                            break;
                        // z compress
                        case EAxisType.Z:
                            _objectInfo.position.x = _prevObjectInfo.position.x;
                            _objectInfo.position.y = _prevObjectInfo.position.y;
                            break;
                    }
                    break;

                case EAxisType.X:
                    float priorityXPos = (priorityPos.x + priObjColBoundsDivTwo.x) * xDir;
                    _objectInfo.position.x = priorityXPos;
                    _objectInfo.scale.x = Mathf.Min(_objectInfo.scale.x, 1);
                    break;
                case EAxisType.Y:
                    float priorityYPos = (priorityPos.y + priObjColBoundsDivTwo.y) * yDir;
                    _objectInfo.position.y = priorityYPos;
                    _objectInfo.scale.y = Mathf.Min(_objectInfo.scale.y, 1);
                    break;
                case EAxisType.Z:
                    float priorityZPos = (priorityPos.z + priObjColBoundsDivTwo.z) * zDir;
                    _objectInfo.position.z = priorityZPos;
                    _objectInfo.scale.z = Mathf.Min(_objectInfo.scale.z, 1);
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,_colCheckDistance);
    }
}
