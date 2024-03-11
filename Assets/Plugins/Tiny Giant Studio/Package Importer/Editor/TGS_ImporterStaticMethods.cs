using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static TinyGiantStudio.PackageImporter.TGS_Package;

namespace TinyGiantStudio.PackageImporter
{
    internal class TGS_ImporterStaticMethods : MonoBehaviour
    {
        public static TGS_Asset[] GetAllTGSAssets()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(TGS_Asset).Name);  //FindAssets uses tags check documentation for more info
            TGS_Asset[] assets = new TGS_Asset[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<TGS_Asset>(path);
            }

            for (int i = 0; i < assets.Length; i++)
            {
                assets[i].UpdateList();
            }

            return assets;
        }


        #region Pipeline stuff
        //Gets Current Pipeline
        public static TGS_Package.Pipeline GetCurrentPipeline()
        {
            //Checks if there is any Render Pipeline asset in the graphics settings
            if (GraphicsSettings.currentRenderPipeline)
            {
                //Gets the type name of the Render Pipeline asset
                string pipelineAssetName = GraphicsSettings.currentRenderPipeline.GetType().Name;

                //Validates if its really a pipeline asset
                if (pipelineAssetName.EndsWith("RenderPipelineAsset"))
                {
                    //If Pipeline asset name starts with HD it means its High Defination Render Pipeline
                    if (pipelineAssetName.StartsWith("HD"))
                    {
                        //Marks current pipeline as High Defination Render Pipeline
                        return TGS_Package.Pipeline.HDRP;
                    }
                    //If Pipeline asset name starts with Universal it means its Universal Render Pipeline
                    else if (pipelineAssetName.StartsWith("Universal"))
                    {
                        //Marks current pipeline as Universal Render Pipeline
                        return TGS_Package.Pipeline.URP;
                    }
                }
                else //If no
                {
                    //Marks current pipeline as Standard Render Pipeline
                    return TGS_Package.Pipeline.SRP;
                }
            }

            //If it has no Render Pipeline asset then
            //Marks current pipeline as Standard Render Pipeline
            return TGS_Package.Pipeline.SRP;
        }

        /// <summary>
        /// Only call this if target and current pipeline is same
        /// </summary>
        /// <param name="currentAndTargetPipeline">Is it target or current package </param>
        /// <param name="minimumPipelineVersionRequired"></param>
        /// <param name="maximumPipelineVersionAllowed"></param>
        /// <returns></returns>
        public static bool ValidPipelineVersion(Pipeline currentAndTargetPipeline, int[] minimumPipelineVersionRequired, int[] maximumPipelineVersionAllowed)
        {
            int[] currentPipelineVersion = PipelineVersion(currentAndTargetPipeline); //current installed version

            bool validMinimumPipelineVersion = ValidMinimumPipelineVersion(currentPipelineVersion, minimumPipelineVersionRequired);
            bool validMaximumPipelineVersion = ValidMaximumPipelineVersion(currentPipelineVersion, maximumPipelineVersionAllowed);

            if (validMinimumPipelineVersion && validMaximumPipelineVersion)
                return true;

            return false;
        }

        /// <summary>
        /// Here int[] = int major version, int minor version of the package
        /// </summary>
        static bool ValidMinimumPipelineVersion(int[] currentPipelineVersion, int[] minimumPipelineVersionRequired)
        {
            if (minimumPipelineVersionRequired == null || currentPipelineVersion == null)
                return true;

            if (minimumPipelineVersionRequired.Length == 0 || currentPipelineVersion.Length == 0)
                return true;

            if (minimumPipelineVersionRequired[0] == 0)
                return true;

            if (currentPipelineVersion[0] == minimumPipelineVersionRequired[0]) //same major version
            {
                if (currentPipelineVersion[1] < minimumPipelineVersionRequired[1]) //but lower minor version than required
                    return false;
            }

            if (currentPipelineVersion[0] < minimumPipelineVersionRequired[0]) //less than required minimum major version
                return false;

            return true;
        }

        static bool ValidMaximumPipelineVersion(int[] currentPipelineVersion, int[] maximumPipelineVersionAllowed)
        {
            if (maximumPipelineVersionAllowed == null || currentPipelineVersion == null)
                return true;

            if (maximumPipelineVersionAllowed.Length == 0 || currentPipelineVersion.Length == 0)
                return true;

            if (maximumPipelineVersionAllowed[0] == 0)
                return true;

            if (currentPipelineVersion[0] == maximumPipelineVersionAllowed[0]) //same major version
            {
                if (currentPipelineVersion[1] > maximumPipelineVersionAllowed[1]) //but higher minor version that accepted
                    return false;
            }

            if (currentPipelineVersion[0] > maximumPipelineVersionAllowed[0]) //more than allowed max version(major)
                return false;

            return true;
        }
        static int[] PipelineVersion(Pipeline pipeline)
        {
            string pipelineBundleIdentifier = GetPipelineBundleIdentifier(pipeline);
            var package = TGS_PackageManagerAgent.GetPackage(pipelineBundleIdentifier);
            if (package != null)
            {
                var versions = package.version.Split('.');
                return Array.ConvertAll(versions, s => int.Parse(s));
            }
            else
                return new int[] { 0, 0 };
        }


        /// <summary>
        /// Doesnt work with SRP
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetPipelineBundleIdentifier(Pipeline pipeline)
        {
            //srp doesnt call it
            string packageID;
            switch (pipeline)
            {
                case TGS_Package.Pipeline.URP:
                    packageID = "com.unity.render-pipelines.universal";
                    break;
                //case TGSPackageAsset.Pipeline.HDRP:
                default:
                    packageID = "com.unity.render-pipelines.high-definition";
                    break;
            }

            return packageID;
        }
        #endregion Pipeline staff



        #region Unity version
        static bool ValidUnityVersion(float minimumUnityVersion, float maximumUnityVersion)
        {
            return false;
        }
        #endregion
    }
}