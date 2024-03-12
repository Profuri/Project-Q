using System.Collections.Generic;
using UnityEditor;

namespace TinyGiantStudio.PackageImporter
{
    /// <summary>
    /// https://docs.unity3d.com/ScriptReference/AssetDatabase-importPackageCompleted.html
    /// 
    /// Used to process all callbacks related to importing packages
    /// </summary>
    /// 
    [InitializeOnLoad]
    public class TGS_AssetDatabaseHandler
    {
        static TGS_AssetDatabaseHandler()
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
        }

        static bool AutoCheckForUpdates() => EditorPrefs.GetBool("AutoCheckUpdates");

        static void OnImportPackageCompleted(string packagename)
        {
            if (!AutoCheckForUpdates())
                return;

            TGS_Package.Pipeline pipeline = TGS_ImporterStaticMethods.GetCurrentPipeline();
            TGS_Asset[] assets = TGS_ImporterStaticMethods.GetAllTGSAssets();

            bool updateAvailable = false;

            for (int i = 0; i < assets.Length; i++)
            {
                if (pipeline == TGS_Package.Pipeline.HDRP)
                    updateAvailable = UpdateAvailable(assets[i].HDRPPackageAssets);
                else if (pipeline == TGS_Package.Pipeline.URP)
                    updateAvailable = UpdateAvailable(assets[i].URPPackageAssets);
                else //if (pipeline == TGS_Package.Pipeline.SRP)
                    updateAvailable = UpdateAvailable(assets[i].SRPPackageAssets);

                if (updateAvailable)
                    break;
            }

            if (updateAvailable)
            {
                TGS_PackageImporterWindow.ShowWindow();
            }
        }

        static bool UpdateAvailable(List<TGS_Package> packages)
        {
            for (int j = 0; j < packages.Count; j++)
            {
                if (packages[j].ignoreUpdates)
                    continue;

                if (packages[j].hasPipelineDependancy)
                {
                    if (!TGS_ImporterStaticMethods.ValidPipelineVersion(packages[j].targetPipeline, packages[j].minimumPipelineVersion, packages[j].maximumPipelineVersion))
                        return false;
                }

                if (packages[j].hasUnityVersionDependancy)
                {

                }

                if (packages[j].currentVersion != packages[j].GetLastInstalledVersion())
                    return true;
            }
            return false;
        }
    }
}