using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace StageStructureConvertSystem
{
    public class StructureObjectUnit : MonoBehaviour, IStructureObject
    {
        [SerializeField] private EStructureObjectType _objectType;

        private ObjectInfo _prevObjectInfo;
        private ObjectInfo _objectInfo;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public void Init()
        {
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

        public void ConvertDimension(EAxisType axisType)
        {
            _objectInfo.position = transform.localPosition;
            _objectInfo.scale = transform.localScale;

            ConvertMesh(axisType);
            TransformSynchronization(axisType);
            ApplyCollider(axisType);
            ObjectSetting(_objectInfo);
        }

        public void ConvertMesh(EAxisType axisType)
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

                if (_objectType == EStructureObjectType.PLATFORM)
                {
                    _objectInfo.mesh = new Mesh
                    {
                        vertices = _prevObjectInfo.mesh.vertices,
                        triangles = _prevObjectInfo.mesh.triangles,
                        normals = _prevObjectInfo.mesh.normals,
                        tangents = _prevObjectInfo.mesh.tangents,
                        bounds = _prevObjectInfo.mesh.bounds,
                        uv = _prevObjectInfo.mesh.uv
                    };
                    
                    var vertices = _prevObjectInfo.mesh.vertices;

                    for (var i = 0; i < vertices.Length; i++)
                    {
                        switch (axisType)
                        {
                            case EAxisType.X:
                                vertices[i].x = 0f;
                                break;
                            case EAxisType.Y:
                                vertices[i].y = 0.5f;
                                break;
                            case EAxisType.Z:
                                vertices[i].z = -0.5f;
                                break;
                        }
                    }
                    
                    _objectInfo.mesh.vertices = vertices;
                }
            }
        }

        public void TransformSynchronization(EAxisType axisType)
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
                    _objectInfo.position.x = _prevObjectInfo.scale.x / 2;
                    _objectInfo.scale.x = 1;
                    break;
                case EAxisType.Y:
                    _objectInfo.position.y = -_prevObjectInfo.scale.y / 2;
                    _objectInfo.scale.y = 1;
                    break;
                case EAxisType.Z:
                    _objectInfo.position.z = -_prevObjectInfo.scale.z / 2;
                    _objectInfo.scale.z = 1;
                    break;
            }
        }

        public void ApplyCollider(EAxisType axisType)
        {
            if (axisType == EAxisType.NONE)
            {
                // _objectInfo.collider = new BoxCollider();
            }
            else
            {
                
            }
        }

        public void ObjectSetting(ObjectInfo info)
        {
            transform.localPosition = info.position;
            transform.localScale = info.scale;

            if (_meshFilter)
            {
                _meshFilter.mesh = info.mesh;
            }

            if (_meshRenderer)
            {
                _meshRenderer.material = info.material;
            }

            // if (transform.TryGetComponent<Collider>(out var collider))
            // {
            //     Destroy(collider);
            // }
            
            // var originalType = info.collider.GetType();
            // var compo = transform.AddComponent<Collider>();
            //
            // var fields = originalType.GetFields();
            // foreach (var field in fields)
            // {
            //     field.SetValue(compo, field.GetValue(info.collider));
            // }
        }
    }
}