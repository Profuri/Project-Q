using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(PoolManager))]
public class PoolManagerEditor : Editor
{
    public PoolManager manager;
    private static string s_folderPath = "Assets/07.ScriptableObjects/PoolingList/";


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
        List<PoolingList> poolingListList = new List<PoolingList>();

        PoolingList poolingList = ScriptableObject.CreateInstance<PoolingList>();

        string assetPath = s_folderPath + "PoolingList.asset";
        if (!AssetDatabase.IsValidFolder(s_folderPath))
        {
            PoolingList originList = AssetDatabase.LoadAssetAtPath<PoolingList>(assetPath);
            if(originList != null)
            {
                poolingList = originList;
            }
        }


        AssetDatabase.CreateAsset(poolingList, assetPath);

        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                Debug.Log("Loaded prefab: " + prefab.name);

                if(prefab.TryGetComponent(out PoolableMono poolableMono))
                {
                    if (poolableMono.poolingCnt <= 0) continue; 
                    PoolingItem poolingItem = new PoolingItem { prefab = poolableMono, cnt =  poolableMono.poolingCnt};
                    poolingList.poolingItems.Add(poolingItem);
                }
            }
        }
        poolingListList.Add(poolingList);
        manager.SettingPoolinglist(poolingListList);
        EditorUtility.SetDirty(poolingList);
    }
}
#endif