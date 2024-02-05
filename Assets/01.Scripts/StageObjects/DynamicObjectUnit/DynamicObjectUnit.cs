using AxisConvertSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DynamicObjectUnit : ObjectUnit
{
    

    private Rigidbody _rigidbody;

    public override void Init(AxisConverter converter)
    {
        _rigidbody = GetComponent<Rigidbody>();
        base.Init(converter);
    }

    public void TransformSynchronization(AxisType axisType)
    {
        // base.TransformSynchronization(axisType);

        // switch (axisType)
        // {
            // case AxisType.None:
                // CheckObject(_prevObjectInfo.CompressType);
                // break;
            // case AxisType.X:
                // _objectInfo.LocalPos.x = 1f;
                // break;
            // case AxisType.Y:
                // _objectInfo.LocalPos.y = 1f;
                // break;
            // case AxisType.Z:
                // _objectInfo.LocalPos.z = -1f;
                // break;
        // }
    }

    // private void CheckObject(AxisType axisType)
    // {
        // var origin = _prevObjectInfo.LocalPos + Vector3.up * (_prevObjectInfo.LocalScale.y / 2f);
        // var dir = Vector3.down;

        // var isHit = Physics.Raycast(origin, dir, out var hit, _rayDistance, _standableObjectMask);

        // if (!isHit)
        // {
            // return;
        // }

        // if (!hit.collider.TryGetComponent<ObjectUnit>(out var unit))
        // {
            // return;
        // }

        // switch (axisType)
        // {
            // case AxisType.X:
                // if (_objectInfo.LocalPos.x >= unit.UnitInfo.LocalPos.x - unit.UnitInfo.LocalScale.x / 2f  &&
                    // _objectInfo.LocalPos.x <= unit.UnitInfo.LocalPos.x + unit.UnitInfo.LocalScale.x / 2f)
                // {
                    // return;
                // }
                // _objectInfo.LocalPos.x = unit.UnitInfo.LocalPos.x;
                // break;
            // case AxisType.Y:
                // if (_objectInfo.LocalPos.y >= unit.UnitInfo.LocalPos.y + unit.UnitInfo.LocalScale.y / 2f + _objectInfo.LocalScale.y / 2f)
                // {
                    // return;
                // }
                // _objectInfo.LocalPos.y = unit.UnitInfo.LocalPos.y + unit.UnitInfo.LocalScale.y / 2f + _objectInfo.LocalScale.y / 2f;
                // break;
            // case AxisType.Z:
                // if (_objectInfo.LocalPos.z >= unit.UnitInfo.LocalPos.z - unit.UnitInfo.LocalScale.z / 2f  &&
                    // _objectInfo.LocalPos.z <= unit.UnitInfo.LocalPos.z + unit.UnitInfo.LocalScale.z / 2f)
                // {
                    // return;
                // }
                // _objectInfo.LocalPos.z = unit.UnitInfo.LocalPos.z;
                // break;
        // }
    // }

    // public override void ReloadObject()
    // {
        // base.ReloadObject();
        // _rigidbody.velocity = Vector3.zero;
    // }
}