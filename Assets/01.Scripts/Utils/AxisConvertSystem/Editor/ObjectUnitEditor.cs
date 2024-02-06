using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace AxisConvertSystem.Editor
{
    [CustomEditor(typeof(ObjectUnit))]
    public class ObjectUnitEditor : UnityEditor.Editor
    {
        private ObjectUnit _target;

        public void OnEnable()
        {
            _target = (ObjectUnit)target;
            ReloadObject();
        }

        public void OnDisable()
        {
            ReloadObject();
        }

        public override void OnInspectorGUI()
        {
            var inspectorObj = _target.customInspector;

            inspectorObj.compressLayer =
                (CompressLayer)EditorGUILayout.EnumPopup("Compress Layer", inspectorObj.compressLayer);
            inspectorObj.staticUnit = EditorGUILayout.Toggle("Static Unit", inspectorObj.staticUnit);
            
            if (!inspectorObj.staticUnit)
            {
                GUILayout.Space(10);
                GUILayout.Label("For Dynamic Object");
                inspectorObj.canStandMask = EditorGUILayout.LayerField("Can Stand Mask", inspectorObj.canStandMask);
                inspectorObj.rayDistance = EditorGUILayout.FloatField("Ray Distance", inspectorObj.rayDistance);
            }
            
            GUILayout.Space(20);

            if (GUILayout.Button("Reload Object"))
            {
                ReloadObject();
            }
        }

        private void ReloadObject()
        {
            if (!_target.transform.Find("Collider"))
            {
                var colObj = new GameObject("Collider");
                colObj.transform.SetParent(_target.transform);
                colObj.transform.Reset();

                if (_target.TryGetComponent<Collider>(out var col))
                {
                    if (col is CharacterController characterController)
                    {
                        var capsuleCol = colObj.AddComponent<CapsuleCollider>();
                        capsuleCol.center = characterController.center;
                        capsuleCol.radius = characterController.radius;
                        capsuleCol.height = characterController.height;
                    }
                    else
                    {
                        var boxCol = colObj.AddComponent<BoxCollider>();
                        boxCol.center = Vector3.zero;
                        boxCol.size = Vector3.one;
                    }
                    
                    DestroyImmediate(col);
                }
                else
                {
                    var boxCol = colObj.AddComponent<BoxCollider>();
                    boxCol.center = Vector3.zero;
                    boxCol.size = Vector3.one;
                }
            }
            
            if (_target.customInspector.staticUnit)
            {
                if (_target.TryGetComponent<Rigidbody>(out var rigid))
                {
                    DestroyImmediate(rigid);
                }
            }
            else
            {
                if (!_target.TryGetComponent<Rigidbody>(out var rigid))
                {
                    _target.transform.AddComponent<Rigidbody>();
                }
            }
        }
    }
}