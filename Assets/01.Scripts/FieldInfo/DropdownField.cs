using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownField : TestField
{
    private TMP_Dropdown _dropdown;
    private Type _type;

    public override object GetValue()
    {
        EAxisType value = (EAxisType)Enum.Parse(typeof(EAxisType), _dropdown.options[_dropdown.value].text);
        return value;
    }

    public override void Init(Type type)
    {
        _type = type;
        _dropdown = GetComponent<TMP_Dropdown>();
         foreach (Enum value in Enum.GetValues(type))
         {
             TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
             optionData.text = $"{value.ToString()}";
             _dropdown.options.Add(optionData);
         }
    }
}
