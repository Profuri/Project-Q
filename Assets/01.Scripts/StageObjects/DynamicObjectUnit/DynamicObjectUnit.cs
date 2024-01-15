using StageStructureConvertSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DynamicObjectUnit : StructureObjectUnitBase
{
    [SerializeField] private LayerMask _standableObjectMask;
    [SerializeField] private float _rayDistance;

    private Rigidbody _rigidbody;

    public override void Init(StructureConverter converter)
    {
        _rigidbody = GetComponent<Rigidbody>();
        base.Init(converter);
    }

    public override void TransformSynchronization(EAxisType axisType)
    {
        base.TransformSynchronization(axisType);

        switch (axisType)
        {
            case EAxisType.NONE:
                CheckObject(_prevObjectInfo.axis);
                break;
            case EAxisType.X:
                _objectInfo.position.x = 1f;
                break;
            case EAxisType.Y:
                _objectInfo.position.y = 1f;
                break;
            case EAxisType.Z:
                _objectInfo.position.z = -1f;
                break;
        }
    }

    private void CheckObject(EAxisType axisType)
    {
        var origin = _prevObjectInfo.position + Vector3.up * (_prevObjectInfo.scale.y / 2f);
        var dir = Vector3.down;

        var isHit = Physics.Raycast(origin, dir, out var hit, _rayDistance, _standableObjectMask);

        if (!isHit)
        {
            return;
        }

        if (!hit.collider.TryGetComponent<StructureObjectUnitBase>(out var unit))
        {
            return;
        }

        switch (axisType)
        {
            case EAxisType.X:
                if (_objectInfo.position.x >= unit.ObjectInfo.position.x - unit.ObjectInfo.scale.x / 2f  &&
                    _objectInfo.position.x <= unit.ObjectInfo.position.x + unit.ObjectInfo.scale.x / 2f)
                {
                    return;
                }
                _objectInfo.position.x = unit.ObjectInfo.position.x;
                break;
            case EAxisType.Y:
                if (_objectInfo.position.y >= unit.ObjectInfo.position.y + unit.ObjectInfo.scale.y / 2f + _objectInfo.scale.y / 2f)
                {
                    return;
                }
                _objectInfo.position.y = unit.ObjectInfo.position.y + unit.ObjectInfo.scale.y / 2f + _objectInfo.scale.y / 2f;
                break;
            case EAxisType.Z:
                if (_objectInfo.position.z >= unit.ObjectInfo.position.z - unit.ObjectInfo.scale.z / 2f  &&
                    _objectInfo.position.z <= unit.ObjectInfo.position.z + unit.ObjectInfo.scale.z / 2f)
                {
                    return;
                }
                _objectInfo.position.z = unit.ObjectInfo.position.z;
                break;
        }
    }

    public override void ReloadObject()
    {
        base.ReloadObject();
        _rigidbody.velocity = Vector3.zero;
    }
}