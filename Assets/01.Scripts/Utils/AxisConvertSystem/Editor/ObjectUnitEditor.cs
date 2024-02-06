using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AxisConvertSystem.Editor
{
    [CustomEditor(typeof(ObjectUnit), true)]
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
            base.OnInspectorGUI();

            if (!_target.staticUnit)
            {
                GUILayout.Space(10);
                GUILayout.Label("For Dynamic Object");

                var layers = new List<string>();
                for (var i = 0; i < 32; ++i)
                {
                    var layer = LayerMask.LayerToName(i);
                    if (string.IsNullOrEmpty(layer))
                    {
                        continue;
                    }
                    layers.Add(layer);
                }
                _target.canStandMask = EditorGUILayout.MaskField("Can Stand Mask",_target.canStandMask, layers.ToArray());
                _target.rayDistance = EditorGUILayout.FloatField("Ray Distance", _target.rayDistance);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Reload Object"))
            {
                ReloadObject();
            }
        }

        private void ReloadObject()
        {
            if (_target == null)
            {
                return;
            }
        
            if (!_target.transform.Find("Collider"))
            {
                var colObj = new GameObject("Collider");
                colObj.transform.SetParent(_target.transform);
                colObj.transform.Reset();
        
                if (_target.TryGetComponent<Collider>(out var col))
                {
                    if (col is CharacterController characterController)
                    {
                        var characterCon = colObj.AddComponent<CharacterController>();
                        characterCon.center = characterController.center;
                        characterCon.radius = characterController.radius;
                        characterCon.height = characterController.height;
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
            
            if (_target.staticUnit)
            {
                if (_target.TryGetComponent<Rigidbody>(out var rigid))
                {
                    DestroyImmediate(rigid);
                }
            }
            else
            {
                if (!_target.TryGetComponent<Rigidbody>(out var rigid) &&
                    _target.compressLayer != CompressLayer.Player)
                {
                    _target.gameObject.AddComponent<Rigidbody>();
                }
            }
        }
    }
}