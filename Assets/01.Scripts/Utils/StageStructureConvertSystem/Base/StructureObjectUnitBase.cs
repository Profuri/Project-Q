using UnityEngine;

namespace StageStructureConvertSystem
{
    [RequireComponent(typeof(Outline))]
    public class StructureObjectUnitBase : MonoBehaviour, IStructureObject
    {
        private Vector3 _originPos;
        private Vector3 _originScale;

        private StructureConverter _converter;

        protected ObjectInfo _prevObjectInfo;
        protected ObjectInfo _objectInfo;

        protected MeshRenderer _meshRenderer;
        protected Material _material;
        protected Collider _collider;
        protected Outline _outline;

        public ObjectInfo PrevObjectInfo => _prevObjectInfo;
        public ObjectInfo ObjectInfo => _objectInfo;

        [SerializeField] private bool _materialRenderSetting = true;
        [SerializeField] private bool _colliderSetting = true;
        [SerializeField] private bool _outlineSetting = true;
        [SerializeField] private bool _interatableYAxis = false;

        public virtual void Init(StructureConverter converter)
        {
            _originPos = transform.localPosition;
            _originScale = transform.localScale;

            _converter = converter;

            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
            _outline = GetComponent<Outline>();
            _outline.enabled = false;

            if (_meshRenderer)
            {
                _material = _meshRenderer.material;
            }

            _objectInfo.position = _originPos;
            _objectInfo.scale = _originScale;
            _objectInfo.axis = EAxisType.NONE;
        }

        public virtual void ConvertDimension(EAxisType axisType)
        {
            _objectInfo.position = transform.localPosition;
            _objectInfo.scale = transform.localScale;
            SwappingObjectInfo(axisType);
        }

        private void SwappingObjectInfo(EAxisType axisType)
        {
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
            if (axisType == EAxisType.NONE)
            {
                PrevTransformSynchronization(_prevObjectInfo.axis);
            }
            else
            {
                NextTransformSynchronization(axisType);
            }
        }

        private void NextTransformSynchronization(EAxisType axisType)
        {
            switch (axisType)
            {
                case EAxisType.X:
                    _objectInfo.position.x = 0;
                    _objectInfo.scale.x = Mathf.Min(_objectInfo.scale.x, 1);
                    break;
                case EAxisType.Y:
                    _objectInfo.position.y = 1;
                    _objectInfo.scale.y = Mathf.Min(_objectInfo.scale.y, 1);
                    break;
                case EAxisType.Z:
                    _objectInfo.position.z = 0;
                    _objectInfo.scale.z = Mathf.Min(_objectInfo.scale.z, 1);
                    break;
            }
        }

        private void PrevTransformSynchronization(EAxisType prevAxis)
        {
            switch (prevAxis)
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
        }

        public virtual void ObjectSetting()
        {
            MaterialRenderSetting();
            ColliderSetting();
            OutlineSetting();
            
            transform.localPosition = _objectInfo.position;
            transform.localScale = _objectInfo.scale;

            if (_meshRenderer)
            {
                _meshRenderer.material = _material;
            }
        }

        public virtual void ReloadObject()
        {
            Debug.Log("ReloadPoint : " + StageManager.Instance.ReloadPoint);
            _objectInfo.position = StageManager.Instance.ReloadPoint;
            _objectInfo.scale = _originScale;
            TransformSynchronization(_converter.AxisType);
            ObjectSetting();
        }

        protected virtual void MaterialRenderSetting()
        {
            if (!_material || !_materialRenderSetting)
            {
                return;
            }
            
            switch (_objectInfo.axis)
            {
                case EAxisType.X:
                    _material.renderQueue = Mathf.CeilToInt(_prevObjectInfo.position.x + 5f);
                    break;
                case EAxisType.Y:
                    _material.renderQueue = Mathf.CeilToInt(_prevObjectInfo.position.y);
                    break;
                case EAxisType.Z:
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

            _collider.isTrigger = _objectInfo.axis == EAxisType.Y && _interatableYAxis;
        }

        private void OutlineSetting()
        {
            if (_objectInfo.axis != EAxisType.Y || !_outlineSetting)
            {
                return;
            }

            _outline.enabled = !_interatableYAxis;
        }

        public void RemoveUnit()
        {
            _converter.RemoveObject(this);
        }

        //public void ReSetOriginPos(Vector3 pos)
        //{
        //    _originPos = pos;
        //    _originScale = transform.localScale;

        //    _objectInfo.position = _originPos;
        //    _objectInfo.scale = _originScale;
        //    _objectInfo.axis = EAxisType.NONE;
        //}
    }
}