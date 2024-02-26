using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
namespace Fabgrid
{
    public static class FieldInfoStorage
    {
        private static Dictionary<int,FieldInfo> s_fieldInfoDictionary;
        private static int _fieldIID;

        public static void SetFieldInfo(IProvidableFieldInfo providable)
        {
            s_fieldInfoDictionary.Clear();
            _fieldIID = 0;
            List<FieldInfo> fieldInfoList = providable.GetFieldInfos();

            foreach (FieldInfo fieldInfo in fieldInfoList)
            {
                s_fieldInfoDictionary.Add(_fieldIID,fieldInfo);
            }
        }
         
        public static List<FieldInfo> GetFieldInfo()
        {
            return s_fieldInfoDictionary.Values.ToList();
        }
    }
}
