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

        private List<string> _layers;
        private List<int> _layerNumbers;
        
        public void OnEnable()
        {
            _target = (ObjectUnit)target;

            _layers = new List<string>();
            _layerNumbers = new List<int>();
            
            for (var i = 0; i < 32; ++i)
            {
                var layer = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layer))
                {
                    _layers.Add(layer);
                    _layerNumbers.Add(i);
                }
            }
            
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
                
                _target.canStandMask.value = GetLayerMaskField();
                _target.rayDistance = EditorGUILayout.FloatField("Ray Distance", _target.rayDistance);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Reload Object"))
            {
                ReloadObject();
            }
        }

        private int GetLayerMaskField()
        {
            var maskWithoutEmpty = 0;
            for (var i = 0; i < _layerNumbers.Count; ++i)
            {
                if ((_target.canStandMask & (1 << _layerNumbers[i])) > 0)
                {
                    maskWithoutEmpty |= 1 << i;
                }
            }
            maskWithoutEmpty = EditorGUILayout.MaskField("Can Stand Mask", maskWithoutEmpty, _layers.ToArray());
            var mask = 0;
            for (var i = 0; i < _layerNumbers.Count; ++i)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                {
                    mask |= 1 << _layerNumbers[i];
                }
            }
            return mask;
        }

        private void ReloadObject()
        {
            if (_target == null || _target.compressLayer == CompressLayer.Player)
            {
                return;
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
                if (!_target.TryGetComponent<Rigidbody>(out var rigid))
                {
                    _target.gameObject.AddComponent<Rigidbody>();
                }
            }
        }
    }
}