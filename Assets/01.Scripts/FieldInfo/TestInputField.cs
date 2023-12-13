using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestInputField : TestField
{
    private TMP_InputField _inputField;


    public override object GetValue()
    {
        return null;
    }

    public override void Init(Type type)
    {
        _inputField = GetComponent<TMP_InputField>();
    }
}
