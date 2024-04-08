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
            if (target is not ObjectUnit)
            {
                return;
            }
            
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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUILayout.Space(20);
            
            GUILayout.Label("Default Unit Setting");
            VariableLoad(ref _target.compressLayer, (CompressLayer)EditorGUILayout.EnumPopup("Compress Layer", _target.compressLayer));
            VariableLoad(ref _target.renderType, (UnitRenderType)EditorGUILayout.EnumPopup("Render Target", _target.renderType));
            VariableLoad(ref _target.staticUnit, EditorGUILayout.Toggle("Static Unit", _target.staticUnit));
            VariableLoad(ref _target.climbableUnit, EditorGUILayout.Toggle("Climbable Unit", _target.climbableUnit));
            VariableLoad(ref _target.activeUnit, EditorGUILayout.Toggle("Active Unit", _target.activeUnit));
            VariableLoad(ref _target.subUnit, EditorGUILayout.Toggle("Sub Unit", _target.subUnit));

            if (!_target.staticUnit)
            {                                      
                GUILayout.Space(10);
                GUILayout.Label("For Dynamic Unit");
                    
                VariableLoad(ref _target.canStandMask, GetLayerMaskField());
                if (_target.canStandMask != 0)
                {
                    VariableLoad(ref _target.checkOffset, EditorGUILayout.Slider("Check Offset", _target.checkOffset, 0f, 1f));
                }
                VariableLoad(ref _target.useGravity, EditorGUILayout.Toggle("Use Gravity", _target.useGravity));
            }
        }

        private void VariableLoad<T>(ref T origin, T variable)
        {
            if (variable != null && !origin.Equals(variable))
            {
                origin = variable;
                EditorUtility.SetDirty(_target);
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
            if (_target is null)
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