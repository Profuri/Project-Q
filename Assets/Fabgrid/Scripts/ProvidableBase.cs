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

        public virtual void SetFieldInfos(List<FieldInfo> infos)
        {
            //이거 나중에 안되면 수정 해야됨.
            if (infos == null) return;
            var fieldInfoList = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            foreach (FieldInfo fieldInfo in fieldInfoList)
            {
                FieldInfo matchInfo = infos.Find(info => info == fieldInfo);
                matchInfo?.SetValue(this,infos);
            }
        }
    }
}
