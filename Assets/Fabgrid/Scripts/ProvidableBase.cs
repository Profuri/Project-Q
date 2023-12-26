using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace Fabgrid
{
    public abstract class ProvidableBase : MonoBehaviour,IProvidableFieldInfo
    {
        public virtual List<FieldInfo> GetFieldInfos()
        {
            List<FieldInfo> resultFieldInfoList = new List<FieldInfo>();

            var fieldInfoList = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            foreach (var fieldInfo in fieldInfoList)
            {
                if (Attribute.IsDefined(fieldInfo, typeof(SerializeField)))
                {
                    resultFieldInfoList.Add(fieldInfo);
                }
            }

            return resultFieldInfoList;
        }
    }
}
