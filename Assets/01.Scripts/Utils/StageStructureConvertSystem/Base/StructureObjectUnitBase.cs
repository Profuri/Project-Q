using UnityEngine;

namespace StageStructureConvertSystem
{
    public class StructureObjectUnitBase : MonoBehaviour, IStructureObject
    {
        private Vector3 _originPos;
        private StructureConverter _converter;
        
        protected ObjectInfo _prevObjectInfo;
        protected ObjectInfo _objectInfo;

        protected Collider _collider;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        
        public ObjectInfo PrevObjectInfo => _prevObjectInfo;
        public ObjectInfo ObjectInfo => _objectInfo;

        public virtual void Init(StructureConverter converter)
        {
            _originPos = transform.position;
            _converter = converter;

            _collider = GetComponent<Collider>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();

            if (_meshFilter)
            {
                _objectInfo.mesh = _meshFilter.mesh;
            }

            if (_meshRenderer)
            {
                _objectInfo.material = _meshRenderer.material;
            }

            _objectInfo.position = transform.localPosition;
            _objectInfo.scale = transform.localScale;
            _objectInfo.axis = EAxisType.NONE;
        }

        public virtual void ConvertDimension(EAxisType axisType)
        {
            _objectInfo.position = transform.localPosition;
            _objectInfo.scale = transform.localScale;

            // convert to 3D
            if (axisType == EAxisType.NONE)
            {
                (_prevObjectInfo, _objectInfo) = (_objectInfo, _prevObjectInfo);
            }
            // convert to 2D
            else
            {
                _prevObjectInfo = _objectInfo;
                _objectInfo.axis = axisType;
            }
        }

        public virtual void TransformSynchronization(EAxisType axisType)
        {
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
                    _objectInfo.position.x = 0;
                    _objectInfo.scale.x = Mathf.Min(_objectInfo.scale.x, 1);
                    break;
                case EAxisType.Y:
                    _objectInfo.position.y = 0;
                    _objectInfo.scale.y = Mathf.Min(_objectInfo.scale.y, 1);
                    break;
                case EAxisType.Z:
                    _objectInfo.position.z = 0;
                    _objectInfo.scale.z = Mathf.Min(_objectInfo.scale.z, 1);
                    break;
            }
        }

        public virtual void ObjectSetting()
        {
            transform.localPosition = _objectInfo.position;
            transform.localScale = _objectInfo.scale;

            if (_meshFilter)
            {
                _meshFilter.mesh = _objectInfo.mesh;
            }

            if (_meshRenderer)
            {
                _meshRenderer.material = _objectInfo.material;
            }
        }

        public virtual void ReloadObject()
        {
            _objectInfo.position = _originPos;
            TransformSynchronization(_converter.AxisType);
            ObjectSetting();
        }
    }
}