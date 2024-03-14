using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomEditor(typeof(PoolManager))]
public class PoolManagerEditor : Editor
{
    public PoolManager manager;

    private void OnEnable()
    {
        manager = target as PoolManager;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Reload"))
        {
            Debug.Log("Reload");
            LoadPoolingList();
        }
    }

    private void LoadPoolingList()
    {
        //get PoolableMono classes
        List<Type> derivedTypeList = GetDerivedClasses(typeof(PoolableMono));

        foreach(Type type in derivedTypeList)
        {
            Debug.Log($"Type: {type}");
        }
    }

    public static List<Type> GetDerivedClasses(Type baseType)
    {
        // 현재 어셈블리(현재 실행되는 어셈블리)에서 모든 타입을 가져옵니다.
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] allTypes = assembly.GetTypes();
        
        foreach(Type type in allTypes)
        {
            Debug.Log($"AllType: {type}");
        }

        // baseType을 상속받는 클래스를 필터링합니다.
        List<Type> derivedTypes = allTypes
            .Where(type => type.IsSubclassOf(baseType))
            .ToList();

        return derivedTypes;
    }
}
