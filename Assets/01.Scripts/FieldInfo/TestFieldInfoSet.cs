using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using StageStructureConvertSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class TestFieldInfoSet : MonoBehaviour
{
    [SerializeField] private TestFieldFactory _factory;
    
    private Type _type;
    private Dictionary<FieldInfo,TestField> _fieldDictionary = new Dictionary<FieldInfo,TestField>();

    private void Awake()
    {
        _type = typeof(PictureUnit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            var fieldInfoList = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            foreach (var fieldInfo in fieldInfoList)
            {
                if (Attribute.IsDefined(fieldInfo, typeof(SerializeField)))
                {
                    var field = _factory.CreateUI(fieldInfo);
                    _fieldDictionary.Add(fieldInfo,field);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            PictureUnit unitBase = new PictureUnit();

            foreach (var kvp in _fieldDictionary)
            {
                //unitBase의 info값을 바꾼다.
                if (kvp.Value != null)
                {
                    kvp.Key.SetValue(unitBase,kvp.Value.GetValue());
                }
            }

            foreach (var fieldinfo in _fieldDictionary.Keys)
            {
                Debug.Log($"value: {fieldinfo.GetValue(unitBase)}");
            }
        }
    }
}
