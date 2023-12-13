using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
public class TestFieldFactory : MonoBehaviour 
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private List<TestField> _fieldList;
    
    
    public TestField CreateUI(FieldInfo fieldInfo)
    {
        //TestField inputField = Instantiate(_testField, Vector3.zero, Quaternion.identity, _canvas).GetComponent<TestField>();
        //inputField.Init(fieldInfo.FieldType);
        Type type = fieldInfo.FieldType;
        TestField field = null;
        if (type.IsEnum)
        {
            field = Instantiate(GetCorrectField<DropdownField>(),Vector3.zero,Quaternion.identity,_canvas);
        }
        else if (type.IsValueType)
        {
            if (type == typeof(float))
            {
                field = Instantiate(GetCorrectField<TestInputField>(), Vector3.zero, Quaternion.identity, _canvas);
            }
        }
        field?.Init(type);
        return field;
    }

    public TestField GetCorrectField<T>() where T : TestField
    {
        foreach (var field in _fieldList)
        {
            if (field.TryGetComponent(out T component))
            {
                return component;
            }
        }
        Debug.LogError("Can't Find correct field");
        return null;
    }
}
