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

            _objectInfo.mesh = _meshFilter.mesh;
            _objectInfo.material = _meshRenderer.material;
            _objectInfo.position = transform.localPosition;
            _objectInfo.scale = transform.localScale;
        }

        public void ConvertDimension(EAxisType axisType)
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
                            vertices[i].x = 0.5f;
                            break;
                        case EAxisType.Y:
                            vertices[i].y = -0.5f;
                            break;
                        case EAxisType.Z:
                            vertices[i].z = -0.5f;
                            break;
                    }
                }
                
                // switch (axisType)
                // {
                //     case EAxisType.X:
                //         Array.Sort(vertices, (v1, v2) => v1.x.CompareTo(v2.x) * -1);
                //         break;
                //     case EAxisType.Y:
                //         Array.Sort(vertices, (v1, v2) => v1.y.CompareTo(v2.x));
                //         break;
                //     case EAxisType.Z:
                //         Array.Sort(vertices, (v1, v2) => v1.z.CompareTo(v2.z));
                //         break;
                // }

                _objectInfo.mesh.vertices = vertices;
            }
            
            TransformSynchronization();
            ApplyCollider(axisType);
            ObjectSetting(_objectInfo);
        }

        public void TransformSynchronization()
        {
            
        }

        public void ApplyCollider(EAxisType axisType)
        {
            if (axisType == EAxisType.NONE)
            {
                _objectInfo.collider = new BoxCollider();
            }
            else
            {
                
            }
        }

        public void ObjectSetting(ObjectInfo info)
        {
            transform.localPosition = info.position;
            transform.localScale = info.scale;

            _meshFilter.mesh = info.mesh;
            _meshRenderer.material = info.material;

            if (transform.TryGetComponent<Collider>(out var collider))
            {
                Destroy(collider);
            }
            
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