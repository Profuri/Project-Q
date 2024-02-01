using UnityEngine;

namespace AxisConvertSystem
{
    [RequireComponent(typeof(Outline))]
    public class StructureObjectUnitBase : MonoBehaviour, IStructureObject
    {
        protected Vector3 _originPos;
        protected Quaternion _originRotation;
        protected Vector3 _originScale;

        private AxisConverter _converter;

        protected ObjectInfo _prevObjectInfo;
        protected ObjectInfo _objectInfo;

        protected Rigidbody _rigidbody;
        protected MeshRenderer _meshRenderer;
        protected Material _material;
        protected Collider _collider;
        protected Outline _outline;

        public ObjectInfo PrevObjectInfo => _prevObjectInfo;
        public ObjectInfo ObjectInfo => _objectInfo;

        [SerializeField] private bool _reloadOnCollisionToObstacle = false;
        public bool ReloadOnCollisionToObstacle => _reloadOnCollisionToObstacle;

        [Header("Property setting toggle")]
        [SerializeField] private bool _materialRenderSetting = true;
        [SerializeField] private bool _colliderSetting = true;
        [SerializeField] private bool _outlineSetting = true;
        [SerializeField] private bool _interatableYAxis = false;
        [SerializeField] private bool _rigidobySetting = false;

        private int _defaultRenderQueue;

        public virtual void Init(AxisConverter converter)
        {
            _originPos = transform.localPosition;
            _originRotation = transform.rotation;
            _originScale = transform.localScale;

            _converter = converter;

            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
            _outline = GetComponent<Outline>();
            _outline.enabled = false;
            _rigidbody = GetComponent<Rigidbody>();

            if (_meshRenderer)
            {
                _material = _meshRenderer.material;
                _defaultRenderQueue = _material.renderQueue;
            }

            _objectInfo.position = _originPos;
            _objectInfo.rotation = _originRotation;
            _objectInfo.scale = _originScale;
            _objectInfo.axis = AxisType.None;
        }

        public virtual void ConvertDimension(AxisType axisType)
        {
            _objectInfo.position = transform.localPosition;
            _objectInfo.rotation = transform.rotation;
            _objectInfo.scale = transform.localScale;
            SwappingObjectInfo(axisType);
        }

        private void SwappingObjectInfo(AxisType axisType)
        {
            // convert to 3D
            if (axisType == AxisType.None)
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

        public virtual void TransformSynchronization(AxisType axisType)
        {
            if (axisType == AxisType.None)
            {
                PrevTransformSynchronization(_prevObjectInfo.axis);
            }
            else
            {
                NextTransformSynchronization(axisType);
            }
        }

        private void NextTransformSynchronization(AxisType axisType)
        {
            switch (axisType)
            {
                case AxisType.X:
                    _objectInfo.position.x = 0;
                    _objectInfo.scale.x = Mathf.Min(_objectInfo.scale.x, 1);
                    break;
                case AxisType.Y:
                    _objectInfo.position.y = 1;
                    _objectInfo.scale.y = Mathf.Min(_objectInfo.scale.y, 1);
                    break;
                case AxisType.Z:
                    _objectInfo.position.z = 0;
                    _objectInfo.scale.z = Mathf.Min(_objectInfo.scale.z, 1);
                    break;
            }
        }

        private void PrevTransformSynchronization(AxisType prevAxis)
        {
            switch (prevAxis)
            {
                // x compress
                case AxisType.X:
                    _objectInfo.position.y = _prevObjectInfo.position.y;
                    _objectInfo.position.z = _prevObjectInfo.position.z;
                    break;
                // y compress
                case AxisType.Y:
                    _objectInfo.position.x = _prevObjectInfo.position.x;
                    _objectInfo.position.z = _prevObjectInfo.position.z;
                    break;
                // z compress
                case AxisType.Z:
                    _objectInfo.position.x = _prevObjectInfo.position.x;
                    _objectInfo.position.y = _prevObjectInfo.position.y;
                    break;
            }
        }

        public virtual void ObjectSetting()
        {
            MaterialRenderSetting();
            ColliderSetting();
            OutlineSetting();
            RigidbodySetting();

            transform.localPosition = _objectInfo.position;
            transform.rotation = _objectInfo.rotation;
            transform.localScale = _objectInfo.scale;

            if (_meshRenderer)
            {
                _meshRenderer.material = _material;
            }
        }

        public void SetObjectInfo(Vector3 localPos, Quaternion rotation, Vector3 scale)
        {
            _objectInfo.position = localPos;
            _objectInfo.rotation = rotation;
            _objectInfo.scale = scale;

            if (_converter.AxisType != AxisType.None)
            {
                TransformSynchronization(_converter.AxisType);
            }
            ObjectSetting();
        }

        public virtual void ReloadObject()
        {
            SetObjectInfo(_originPos, _originRotation, _originScale);
        }

        protected virtual void MaterialRenderSetting()
        {
            if (!_material || !_materialRenderSetting)
            {
                return;
            }
            
            switch (_objectInfo.axis)
            {
                case AxisType.None:
                    _material.renderQueue = _defaultRenderQueue;
                    break;
                case AxisType.X:
                    _material.renderQueue = Mathf.CeilToInt(_prevObjectInfo.position.x + 5f);
                    break;
                case AxisType.Y:
                    _material.renderQueue = Mathf.CeilToInt(_prevObjectInfo.position.y);
                    break;
                case AxisType.Z:
                    _material.renderQueue = Mathf.CeilToInt(Mathf.Abs(_prevObjectInfo.position.z - 5f));
                    break;
            }
        }

        protected virtual void ColliderSetting()
        {
            if (!_collider || !_colliderSetting)
            {
                return;
            }

            _collider.isTrigger = _objectInfo.axis == AxisType.Y && _interatableYAxis;
        }

        private void OutlineSetting()
        {
            if (!_outlineSetting)
            {
                return;
            }

            _outline.enabled = _objectInfo.axis == AxisType.Y && !_interatableYAxis;
        }

        private void RigidbodySetting()
        {
            if (_rigidbody is null || !_rigidobySetting)
            {
                return;
            }
            
            _rigidbody.useGravity = _objectInfo.axis != AxisType.Y;
        }

        public void RemoveUnit()
        {
            _converter.RemoveObject(this);
        }
    }
}