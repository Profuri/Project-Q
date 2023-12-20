using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
public interface IProvidableFieldInfo
{
    public List<FieldInfo> GetFieldInfos();    
}
