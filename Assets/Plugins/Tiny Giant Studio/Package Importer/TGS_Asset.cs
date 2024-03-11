using System.Collections.Generic;
using UnityEngine;

namespace TinyGiantStudio.PackageImporter
{
    /// <summary>
    /// This is an internal scriptable object file. Not meant for end user
    /// </summary>
    [CreateAssetMenu(fileName = "New Asset Importer", menuName = "Tiny Giant Studio/Package Importer/New Asset Importer")]
    internal class TGS_Asset : ScriptableObject
    {
        public string assetName = "Asset name";
        public string publisher = "Publisher";

        [Tooltip("This is an internal scriptable object file. Not meant for end user")]
        public List<TGS_Package> packages = new List<TGS_Package>();

        public List<TGS_Package> SRPPackageAssets { get; private set; } = new List<TGS_Package>();
        public List<TGS_Package> URPPackageAssets { get; private set; } = new List<TGS_Package>();
        public List<TGS_Package> HDRPPackageAssets { get; private set; } = new List<TGS_Package>();
        public List<TGS_Package> AnyPipelinePackageAssets { get; private set; } = new List<TGS_Package>();


        public void UpdateList()
        {
            SRPPackageAssets = packages.FindAll(p => p.targetPipeline == TGS_Package.Pipeline.SRP && p.hasPipelineDependancy);

            URPPackageAssets = packages.FindAll(p => p.targetPipeline == TGS_Package.Pipeline.URP && p.hasPipelineDependancy);

            HDRPPackageAssets = packages.FindAll(p => p.targetPipeline == TGS_Package.Pipeline.HDRP && p.hasPipelineDependancy);

            AnyPipelinePackageAssets = packages.FindAll(p => !p.hasPipelineDependancy);
        }
    }
}