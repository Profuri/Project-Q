using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;


namespace TinyGiantStudio.PackageImporter
{
    internal class TGS_PackageImporterWindow : EditorWindow
    {
        /// <summary>
        /// 
        /// </summary>
        readonly string importerVersion = "1.0.1a";


        private TGS_Asset[] assets;

        //Current installed pipeline
        private TGS_Package.Pipeline currentPipeline;

        //Scroll Area of packages list and rp foldouts
        private Vector2 assetsScrollView;
        private Vector2 packagesScrollView;


        GUIStyle openedAsset;

        GUIStyle assetHeaderStyle = null;
        GUIStyle smallGreyLabel = null;
        GUIStyle centeredGreyLabel = null;


        GUIStyle foldOutStyle = null;
        GUIStyle noteStyle = null;
        GUIStyle paragraphStyle = null;
        GUIStyle assetNotesStyle = null;
        GUIStyle srpHeaderStyle = null;
        GUIStyle toolBarButtonStyle = null;
        GUIStyle warningStyle = null;
        GUIStyle ignoreUpdateStyle = null;

        Color openedFoldoutTitleColor = new Color(136 / 255f, 173 / 255f, 234 / 255f, 1f);
        Color openedFoldoutTitleColor_lightSkin = new Color(123f / 255f, 120f / 255f, 0, 1f);
        Color openedFoldoutTitleColor_darkSkin = new Color(240f / 255f, 241f / 255f, 101f / 255f, 1f);

        AnimBool showAnyPipelinePackages;
        AnimBool showSRPPackages;
        AnimBool showURPPackages;
        AnimBool showHDRPPackages;


        static Texture faviconIcon;
        static Texture titleIcon;

        int selectedAsset = 0;

        bool autoCheckUpdate;
        readonly string checkUpdatePref = "AutoCheckUpdates";


        private void OnAwake()
        {
            TGS_PackageManagerAgent.Check();
        }

        void OnEnable()
        {
            faviconIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Package Importer/Utility/Editor Icons/Favicon.png") as Texture2D;
            titleIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Package Importer/Utility/Editor Icons/TGS Logo.png") as Texture2D;

            assets = TGS_ImporterStaticMethods.GetAllTGSAssets();
            autoCheckUpdate = EditorPrefs.GetBool(checkUpdatePref);
            currentPipeline = TGS_ImporterStaticMethods.GetCurrentPipeline();
            SetAnimBools(); //needs current pipeline
        }

        [MenuItem("Tools/Tiny Giant Studio/Package Importer", false, 102)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow<TGS_PackageImporterWindow>("TGS Package Importer");
            editorWindow.titleContent = new GUIContent(" TGS Package Importer", faviconIcon, "");

            editorWindow.minSize = new Vector2(850, 400);
        }

        void OnGUI()
        {
            GenerateStyle();
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10);
            GUILayout.Label("Please import the right packages for your pipeline.", centeredGreyLabel);
            GUILayout.Space(10);

            if (assets != null)
            {
                if (assets.Length > 0)
                {
                    GUILayout.BeginHorizontal();

                    DrawAssetsList();
                    DrawSelectedAsset();

                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label("No asset found. Please create a new asset importer file to be discovered.", noteStyle);
                    GUILayout.Space(15);
                }
            }
            else
            {
                assets = TGS_ImporterStaticMethods.GetAllTGSAssets();
            }

            BottomInformation();

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(checkUpdatePref, autoCheckUpdate);
            }
        }
        void DrawAssetsList()
        {
            GUILayout.BeginVertical("Box", GUILayout.MinWidth(200), GUILayout.MaxWidth(200));
            assetsScrollView = EditorGUILayout.BeginScrollView(assetsScrollView, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);

            for (int i = 0; i < assets.Length; i++)
            {
                if (selectedAsset != i)
                {
                    GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 1f);
                    GUILayout.BeginVertical(EditorStyles.toolbar);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_Folder Icon"), GUILayout.MaxWidth(15));
                    if (GUILayout.Button(new GUIContent(" " + assets[i].assetName, "File name: " + assets[i].name), EditorStyles.label))
                    {
                        selectedAsset = i;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                    GUILayout.BeginVertical(EditorStyles.toolbar);

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_FolderOpened Icon"), GUILayout.MaxWidth(15));
                    EditorGUILayout.LabelField(new GUIContent(" " + assets[i].assetName, "File name: " + assets[i].name), openedAsset);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }


        private void DrawSelectedAsset()
        {
            packagesScrollView = EditorGUILayout.BeginScrollView(packagesScrollView, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
            int i = selectedAsset;
            //for (int i = 0; i < assets.Length; i++)
            {
                //GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);
                AssetInformation(assets[i]);
                GUILayout.Space(15);

                AnyPipelinePackages(assets[i]);
                GUILayout.Space(10);
                RPPackages(i);
            }
            EditorGUILayout.EndScrollView();
        }

        private void RPPackages(int i)
        {
            if (currentPipeline == TGS_Package.Pipeline.URP)
            {
                URPPackages(assets[i]);
                GUILayout.Space(10);
                HDRPPackages(assets[i]);
                GUILayout.Space(10);
                SRPPackages(assets[i]);
            }
            else if (currentPipeline == TGS_Package.Pipeline.HDRP)
            {
                HDRPPackages(assets[i]);
                GUILayout.Space(10);
                URPPackages(assets[i]);
                GUILayout.Space(10);
                SRPPackages(assets[i]);
            }
            else
            {
                SRPPackages(assets[i]);
                GUILayout.Space(10);
                URPPackages(assets[i]);
                GUILayout.Space(10);
                HDRPPackages(assets[i]);
            }
        }



        void AssetInformation(TGS_Asset asset)
        {
            if (asset == null)
                return;

            GUILayout.Label(new GUIContent(asset.assetName, "File name: " + asset.name), assetHeaderStyle);
            GUILayout.Label(asset.publisher, smallGreyLabel);
        }


        void AnyPipelinePackages(TGS_Asset asset)
        {
            if (asset.AnyPipelinePackageAssets.Count == 0)
                return;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            showAnyPipelinePackages.target = EditorGUILayout.Foldout(showAnyPipelinePackages.target, new GUIContent("Any Pipeline Packages", ""), true, foldOutStyle);
            ImportAllButton(TGS_Package.Pipeline.SRP); //todo
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (EditorGUILayout.BeginFadeGroup(showAnyPipelinePackages.faded))
            {
                GUILayout.BeginVertical("helpbox");

                foreach (var package in asset.AnyPipelinePackageAssets)
                {
                    DrawPacakge(package, false);
                }

                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }
        void SRPPackages(TGS_Asset asset)
        {
            if (asset.SRPPackageAssets.Count == 0)
                return;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            showSRPPackages.target = EditorGUILayout.Foldout(showSRPPackages.target, new GUIContent("Standard Pipeline Packages", ""), true, foldOutStyle);

            if (currentPipeline == TGS_Package.Pipeline.SRP)
                GUILayout.Label("[Current Pipeline]", noteStyle, GUILayout.MaxWidth(110));
            else
                GUILayout.Label("[Wrong Pipeline]", warningStyle, GUILayout.MaxWidth(110));

            ImportAllButton(TGS_Package.Pipeline.SRP);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (EditorGUILayout.BeginFadeGroup(showSRPPackages.faded))
            {
                GUILayout.BeginVertical("helpbox");

                foreach (var package in asset.SRPPackageAssets)
                {
                    if (package.hasPipelineDependancy)
                        DrawPacakge(package, currentPipeline == TGS_Package.Pipeline.SRP);
                }

                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }
        void URPPackages(TGS_Asset asset)
        {
            if (asset.URPPackageAssets.Count == 0)
                return;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            showURPPackages.target = EditorGUILayout.Foldout(showURPPackages.target, new GUIContent("Universal Render Pipeline Packages", ""), true, foldOutStyle);
            if (currentPipeline == TGS_Package.Pipeline.URP)
                GUILayout.Label("[Current Pipeline]", noteStyle, GUILayout.MaxWidth(110));
            else
                GUILayout.Label("[Wrong Pipeline]", warningStyle, GUILayout.MaxWidth(110));
            ImportAllButton(TGS_Package.Pipeline.URP);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (EditorGUILayout.BeginFadeGroup(showURPPackages.faded))
            {
                GUILayout.BeginVertical("helpbox");

                foreach (var package in asset.URPPackageAssets)
                {
                    DrawPacakge(package, currentPipeline == TGS_Package.Pipeline.URP);
                }

                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }
        void HDRPPackages(TGS_Asset asset)
        {
            if (asset.HDRPPackageAssets.Count == 0)
                return;

            EditorGUI.indentLevel = 0;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            showHDRPPackages.target = EditorGUILayout.Foldout(showHDRPPackages.target, new GUIContent("High Definition Render Pipeline Packages", ""), true, foldOutStyle);
            if (currentPipeline == TGS_Package.Pipeline.HDRP)
                GUILayout.Label("[Current Pipeline]", noteStyle, GUILayout.MaxWidth(110));
            else
                GUILayout.Label("[Wrong Pipeline]", warningStyle, GUILayout.MaxWidth(110));
            ImportAllButton(TGS_Package.Pipeline.HDRP);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (EditorGUILayout.BeginFadeGroup(showHDRPPackages.faded))
            {
                GUILayout.BeginVertical("helpbox");

                foreach (var package in asset.HDRPPackageAssets)
                {
                    DrawPacakge(package, currentPipeline == TGS_Package.Pipeline.HDRP);
                }

                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }

        void BottomInformation()
        {
            EditorGUI.indentLevel = 0;
            GUILayout.BeginHorizontal(GUILayout.MaxHeight(50)); //-----------
            GUILayout.Label(titleIcon, GUILayout.MaxHeight(60), GUILayout.MaxWidth(60));


            GUILayout.BeginVertical();
            if (GUILayout.Button("Tiny Giant Studio", srpHeaderStyle))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/45848?aid=1011ljxWe&utm_source=aff");
            }
            //GUILayout.Label("Tiny Giant Studio", srpHeaderStyle);
            GUILayout.Label("Package importer version: " + importerVersion, paragraphStyle);
            GUILayout.Label("Current pipeline: " + currentPipeline + " | Current unity version: " + Application.unityVersion, paragraphStyle);
            GUILayout.EndVertical();
            var v = new GUIContent("Check for updates", "If disabled, when importing assets, it won't check for for new updates.");


            GUILayout.BeginVertical(GUILayout.MaxHeight(50), GUILayout.MaxWidth(156)); //-----------
            GUILayout.FlexibleSpace();
            autoCheckUpdate = EditorGUILayout.Toggle(v, autoCheckUpdate);
            GUILayout.EndVertical();



            GUILayout.EndHorizontal();//---------------------
        }

        void DrawPacakge(TGS_Package asset, bool correctPipeline = false)
        {
            if (asset == null) return;

            EditorGUI.indentLevel = 1;
            bool hasPackage = asset.package != null;

            GUIContent assetTitle;

            if (!string.IsNullOrEmpty(asset.packageName))
                assetTitle = new GUIContent(asset.packageName);
            else if (asset.package != null)
                assetTitle = new GUIContent(" " + asset.package.name);
            else
                assetTitle = new GUIContent("Unnamed asset");


            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField(assetTitle, srpHeaderStyle, GUILayout.MinWidth(200));
            EditorGUILayout.LabelField("v" + asset.currentVersion, ignoreUpdateStyle, GUILayout.MinWidth(50));
            GUILayout.FlexibleSpace();

            if (hasPackage)
            {
                if (autoCheckUpdate)
                    DrawIgnoreUpdatesUI(asset);
                DrawUpgradeAvailability(asset, correctPipeline);
                DrawImportButton(asset);
            }
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = 2;

            if (hasPackage && !string.IsNullOrWhiteSpace(asset.Description))
                EditorGUILayout.LabelField(asset.Description, paragraphStyle);

            if (!hasPackage)
            {
                EditorGUILayout.LabelField("No package selected", warningStyle);
                return;
            }

            string piplelineVersionWarning = PipelineVersionWarning(asset);
            if (!string.IsNullOrEmpty(piplelineVersionWarning))
                EditorGUILayout.LabelField(piplelineVersionWarning, warningStyle);

            string versionWarning = UnityVersionWarning(asset);
            if (!string.IsNullOrEmpty(versionWarning))
            {
                EditorGUILayout.LabelField(versionWarning, warningStyle);
            }


            if (asset.GetLastInstalledVersion() != 0)
                EditorGUILayout.LabelField("Last installed version: " + asset.GetLastInstalledVersion(), ignoreUpdateStyle);

            GUILayout.Space(2);
            EditorGUI.indentLevel = 0;
        }

        void DrawImportButton(TGS_Package asset)
        {
            if (GUILayout.Button("Import", toolBarButtonStyle, GUILayout.MaxWidth(60)))
            {
                if (asset.package != null)
                {
                    ////If the current pipeline is not same as package's target pipeline
                    ////if (currentPipeline != asset.targetPipeline)
                    //if (asset.hasPipelineDependancy && ImporterStaticMethods.ValidPipelineVersion(asset.targetPipeline, asset.minimumPipelineVersion, asset.maximumPipelineVersion))
                    //{
                    //    //Validates if the user wants to import risking errors of non supported packages by the current Render Pipeline
                    //    if (EditorUtility.DisplayDialog("Warning!", asset.targetPipeline.ToString() + " is not installed. Importing non-supported assets might not work properly", "Proceed", "Cancel"))
                    //    {
                    //        //If Cancel then end the method
                    //        AssetDatabase.ImportPackage(AssetDatabase.GetAssetPath(asset.package), true);
                    //        asset.lastInstalledVersion = asset.currentVersion;
                    //    }
                    //}
                    //else
                    //Otherwise import each package without showing whtas inside of them
                    AssetDatabase.ImportPackage(AssetDatabase.GetAssetPath(asset.package), true);
                    asset.UpdateLastInstalledVersion();
                }
                else
                {
                    EditorUtility.DisplayDialog("Package couldn't be found", "The asset packages seems to be missing from it's intended path. Please redownload the asset.\nIf the problem presists after that, please contact support: \nferdowsurasif@gmail.com", "Ok");
                }
            }
        }

        void DrawUpgradeAvailability(TGS_Package asset, bool correctPipeline)
        {
            if (correctPipeline && asset.GetLastInstalledVersion() != asset.currentVersion)
            {
                if (asset.ignoreUpdates)
                    GUILayout.Label("Upgrade available", ignoreUpdateStyle, GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
                else
                    GUILayout.Label("Upgrade available", noteStyle, GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
            }
        }

        void DrawIgnoreUpdatesUI(TGS_Package asset)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbarButton);
            GUIContent ignoreContent = new GUIContent("Ignore Updates", "This ignores update check on asset import. \nSo, when you update the asset and there is a update availabe, that will be ignored.");
            GUILayout.Label(ignoreContent, ignoreUpdateStyle, GUILayout.MaxWidth(90));
            asset.ignoreUpdates = EditorGUILayout.Toggle(GUIContent.none, asset.ignoreUpdates, GUILayout.MaxWidth(30));
            GUILayout.EndHorizontal();
        }

        string PipelineVersionWarning(TGS_Package asset)
        {
            if (!asset.hasPipelineDependancy)
                return "";

            string warnings = "";

            if (asset.targetPipeline != currentPipeline)
                return warnings += "[ Incorrect pipeline ]";

            if (asset.targetPipeline == TGS_Package.Pipeline.SRP)
                return warnings;

            if (asset.targetPipeline == currentPipeline)
            {
                bool rightPipeline = TGS_ImporterStaticMethods.ValidPipelineVersion(asset.targetPipeline, asset.minimumPipelineVersion, asset.maximumPipelineVersion);
                if (!rightPipeline) warnings += "[ Incorrect pipeline version ]";
            }


            return warnings;
        }

        string UnityVersionWarning(TGS_Package asset)
        {
            if (asset.minimumUnityVersion <= 0 && asset.maximumUnityVersion <= 0)
                return "";

            string warnings = "";

            //Get current unity verison's first 2 parts Major and Minor excluding the release and converts to double for comparism
            double currentUnityVersion = Convert.ToDouble(Application.unityVersion.Substring(0, 6));

            //If assets minimum version is less then current unity version then throws a exception even though its not visible
            if (currentUnityVersion != 0 && currentUnityVersion < asset.minimumUnityVersion)
                warnings += "Current unity version :" + currentUnityVersion + " is lower than the minimum " + asset.minimumUnityVersion + " for this package.";

            if (currentUnityVersion != 0 && currentUnityVersion > asset.minimumUnityVersion)
                warnings += "Current unity version " + currentUnityVersion + " is higher than the maxmimum " + asset.maximumUnityVersion + " for this package.";

            return warnings;
        }









        void ImportAllButton(TGS_Package.Pipeline pipeline)
        {
            //TODO
            //commented out because there is no prompt like normal package import if this one is clicked

            //if (GUILayout.Button("Import All", toolBarButtonStyle, GUILayout.MaxWidth(75)))
            //{
            //    //Validates if the user wants to import all packages in side the foldout
            //    if (EditorUtility.DisplayDialog("Are you sure?", $"Proceed to import all {Enum.GetName(typeof(TGSPackageAsset.Pipeline), pipeline)} supported packages.", "Proceed", "Cancel"))
            //    {
            //        //A local list of package that will be used to import
            //        List<TGSPackageAsset> importablePackages = null;

            //        //Checks which pipeline was sent as parameter and assigns importablePackges to their respective package list for each Render Pipeline
            //        switch (pipeline)
            //        {
            //            case TGSPackageAsset.Pipeline.SRP:
            //                importablePackages = importerSettings.SRPPackageAssets;
            //                break;
            //            case TGSPackageAsset.Pipeline.URP:
            //                importablePackages = importerSettings.URPPackageAssets;
            //                break;
            //            case TGSPackageAsset.Pipeline.HDRP:
            //                importablePackages = importerSettings.HDRPPackageAssets;
            //                break;
            //        }

            //        //Iterates trough the importablePackages
            //        for (int i = 0; i < importablePackages.Count; i++)
            //        {
            //            //If the current pipeline is not same as first package's target pipeline of the list.
            //            if (currentPipeline != importablePackages[i].targetPipeline && i == 0)
            //            {
            //                //Validates if the user wants to import risking errors of non supported packages by the current Render Pipeline
            //                if (!EditorUtility.DisplayDialog("Warning!", importablePackages[i].targetPipeline.ToString() + " is not installed. Importing non-supported assets might not work properly", "Proceed", "Cancel"))
            //                {
            //                    //If Cancel then brake the loop
            //                    break;
            //                }
            //            }

            //            //Otherwise import each package without showing whtas inside of them
            //            AssetDatabase.ImportPackage(AssetDatabase.GetAssetPath(importablePackages[i].package), false);
            //        }
            //    }
            //}
        }


        void SetAnimBools()
        {
            if (currentPipeline == TGS_Package.Pipeline.SRP)
                showSRPPackages = new AnimBool(true);
            else
                showSRPPackages = new AnimBool(false);
            showSRPPackages.valueChanged.AddListener(Repaint);

            if (currentPipeline == TGS_Package.Pipeline.URP)
                showURPPackages = new AnimBool(true);
            else
                showURPPackages = new AnimBool(false);
            showURPPackages.valueChanged.AddListener(Repaint);

            if (currentPipeline == TGS_Package.Pipeline.HDRP)
                showHDRPPackages = new AnimBool(true);
            else
                showHDRPPackages = new AnimBool(false);
            showHDRPPackages.valueChanged.AddListener(Repaint);

            showAnyPipelinePackages = new AnimBool(false);
            showAnyPipelinePackages.valueChanged.AddListener(Repaint);
        }

        void GenerateStyle()
        {
            if (EditorGUIUtility.isProSkin)
            {
                openedFoldoutTitleColor = openedFoldoutTitleColor_darkSkin;
            }
            else
            {
                openedFoldoutTitleColor = openedFoldoutTitleColor_lightSkin;
            }

            if (openedAsset == null)
            {
                openedAsset = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 13
                };
                openedAsset.onNormal.textColor = openedFoldoutTitleColor;
            }

            if (assetHeaderStyle == null)
            {
                assetHeaderStyle = new GUIStyle(EditorStyles.largeLabel)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 16
                };
                assetHeaderStyle.normal.textColor = Color.white;
            }
            if (smallGreyLabel == null)
            {
                smallGreyLabel = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontStyle = FontStyle.Italic,
                };
            }
            if (centeredGreyLabel == null)
            {
                centeredGreyLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    fontSize = 16
                };
            }
            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 12
                };
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }

            if (noteStyle == null)
            {
                noteStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontStyle = FontStyle.Italic
                };
                noteStyle.normal.textColor = Color.green;
            }

            if (warningStyle == null)
            {
                warningStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Italic
                };
                warningStyle.normal.textColor = new Color(1, 0.5f, 0.5f);
            }
            if (srpHeaderStyle == null)
            {
                srpHeaderStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontStyle = FontStyle.Bold,
                };
            }
            if (assetNotesStyle == null)
            {
                assetNotesStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    fontStyle = FontStyle.Italic
                };
            }
            if (paragraphStyle == null)
            {
                paragraphStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Italic
                };
            }


            if (toolBarButtonStyle == null)
            {
                toolBarButtonStyle = new GUIStyle(EditorStyles.toolbarButton)
                {
                    fontStyle = FontStyle.BoldAndItalic
                };
            }
            if (ignoreUpdateStyle == null)
            {
                ignoreUpdateStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Italic,
                };
                ignoreUpdateStyle.normal.textColor = new Color(1f, 1f, 1f, 0.5f);
            }

        }
    }
}