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
            base.OnInspectorGUI();
            
            GUILayout.Space(10);

            if (GUILayout.Button("Reload Object"))
            {
                ReloadObject();
            }
        }

        private void ReloadObject()
        {
            if (_target.StaticObject)
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