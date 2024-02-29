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
        private static Dictionary<int,FieldInfo> s_fieldInfoDictionary = new();
        public static void SetFieldInfo(IProvidableFieldInfo providable)
        {
            s_fieldInfoDictionary.Clear();
            
            if (providable == null)
            {
                return;
            }
            
            List<FieldInfo> fieldInfoList = providable.GetFieldInfos();

            foreach (FieldInfo fieldInfo in fieldInfoList)
            {
                int hashCode = fieldInfo.GetHashCode();
                s_fieldInfoDictionary.Add(hashCode,fieldInfo);
            }
        }
         
        public static List<FieldInfo> GetFieldInfo()
        {
            return s_fieldInfoDictionary.Values.ToList();
        }
    }
}
