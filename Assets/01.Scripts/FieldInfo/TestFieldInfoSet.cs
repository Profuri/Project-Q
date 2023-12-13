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
    public List<FieldInfo> FieldInfoList = new List<FieldInfo>();
    [SerializeField] private TestFieldFactory _factory;
    
    private Type _type;
    private TestField _testField;

    private void Awake()
    {
        _type = typeof(AxisLimitPlatformObjectUnit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            FieldInfoList = _type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            foreach (var fieldInfo in FieldInfoList)
            {
                if (Attribute.IsDefined(fieldInfo, typeof(SerializeField)))
                {
                    _testField = _factory.CreateUI(fieldInfo);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            PlatformObjectUnit unitBase = new PlatformObjectUnit();

            foreach (FieldInfo info in FieldInfoList)
            {
                //unitBase의 info값을 바꾼다.
                info.SetValue(unitBase,1f);
            }
        }
    }
}
