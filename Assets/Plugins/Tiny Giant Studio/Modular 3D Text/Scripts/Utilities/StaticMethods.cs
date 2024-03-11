using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TinyGiantStudio.Text
{
    /// <summary>
    /// This class contains different reusable methods for the asset
    /// </summary>
    public class StaticMethods
    {
        /// <summary>
        /// Returns if an item has a List as parent
        /// </summary>
        /// <param name="transform">The transform being checked</param>
        /// <returns></returns>
        public static List GetParentList(Transform transform)
        {
            if (transform.parent == null)
                return null;

            if (transform.parent.GetComponent<List>())
            {
                return transform.parent.GetComponent<List>();
            }
            else return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor Only!
        /// </summary>
        /// <param name="mesh"></param>
        public static void OptimizeMesh(Mesh mesh)
        {
            MeshUtility.Optimize(mesh);
        }

        public static string settingFileLocation = "Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/M3D Editor Settings.asset";
        public static AssetSettings VerifySettings(AssetSettings settings)
        {
            //Debug.Log("Verifying settings");

            if (settings)
                return settings;

            AssetSettings mySettings = EditorGUIUtility.Load(settingFileLocation) as AssetSettings;
            //AssetSettings mySettings = Resources.Load("Modular 3D Text/M Text_Settings") as AssetSettings;
            if (mySettings)
                return mySettings;

            var objects = Resources.FindObjectsOfTypeAll(typeof(AssetSettings));

            if (objects.Length > 1)
            {
                Debug.LogWarning("Multiple MText_Settings files have been found. Please make sure only one exists to avoid unexpected behavior");
                for (int i = 0; i < objects.Length; i++)
                {
                    Debug.Log("Setting file " + (i + 1) + " : " + AssetDatabase.GetAssetPath(objects[i]));
                }
                return (AssetSettings)objects[0];
            }
            else if (objects.Length == 0)
            {
                Debug.Log("Creating editor settings file for Modular 3D Text.");
                AssetSettings asset = ScriptableObject.CreateInstance<AssetSettings>();
                AssetDatabase.CreateAsset(asset, settingFileLocation);
                AssetDatabase.SaveAssets();
                Debug.Log("Editor settings file for Modular 3D Text has been successfully created: " + settingFileLocation + "\nYou can modify settings from Tools>Tiny Giant Studio/Modular 3D Text");
                return asset;
            }
            else
            {
                return (AssetSettings)objects[0];
            }
        }
#endif
    }
}