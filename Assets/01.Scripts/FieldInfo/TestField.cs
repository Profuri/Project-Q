using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public abstract class TestField : MonoBehaviour
{
    public abstract object GetValue(); 
    public abstract void Init(Type type);
}
