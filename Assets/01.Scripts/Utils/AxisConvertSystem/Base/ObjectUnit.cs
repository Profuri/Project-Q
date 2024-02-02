using System;
using UnityEngine;

namespace AxisConvertSystem
{
    public class ObjectUnit : MonoBehaviour
    {
        [SerializeField] private CompressLayer _compressLayer;
        [SerializeField] private bool _reloadOnCollisionToObstacle = false;
        [SerializeField] private bool _staticObject;

        protected AxisConverter Converter { get; private set; }
        protected Collider Collider { get; private set; }

        private ObjectInfo _initInfo;
        private ObjectInfo _prevInfo;
        private ObjectInfo _currentInfo;

        private float _depth;

        public virtual void Init(AxisConverter converter)
        {
            Converter = converter;
            Collider = GetComponent<Collider>();
        }

        public virtual void Update()
        {
            if (!_staticObject)
            {
                DynamicSynchronized();
            }
        }

        public virtual void LateUpdate()
        {
            if (!_staticObject)
            {
                Synchronized();
            }
        }

        public virtual void ConvertDimension(AxisType axisType)
        {
            
        }

        private void DynamicSynchronized()
        {
            var axisDir = Vector3ExtensionMethod.GetAxisDir(Converter.AxisType);
            
        }

        private void Synchronized()
        {
            
        }
    }
}