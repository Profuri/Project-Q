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
        private static Dictionary<int, FieldInfo> s_fieldInfoDictionary = new();
        private static Dictionary<Type, object> s_fieldValueDictionary = new();

        public static void SetValueInfo(Type type, object value = null)
        {
            if (s_fieldValueDictionary.ContainsKey(type))
            {
                s_fieldValueDictionary[type] = value;
            }
            else
            {
                s_fieldValueDictionary.Add(type, value);
                Debug.LogError($"Can't Find Type: {type}");
            }
        }

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

        public static object GetFieldValue(Type type)
        {
            if(s_fieldValueDictionary.ContainsKey(type))
            {
                Debug.Log($"Can't Find Valeue: {type}");
                return s_fieldValueDictionary[type];
            }
            return null;
        }
    }   
}
