using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestToggle : TestField
{
    private Toggle _toggle;
    public override object GetValue()
    {
        return _toggle.isOn;
    }

    public override void Init(Type type)
    {
        _toggle = GetComponent<Toggle>();
    }
}
