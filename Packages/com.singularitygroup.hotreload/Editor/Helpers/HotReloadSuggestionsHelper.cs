using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {

    public enum HotReloadSuggestionKind {
        UnsupportedChanges,
        UnsupportedPackages,
        SymbolicLinks,
        AutoRecompiledWhenPlaymodeStateChanges,
        UnityBestDevelopmentToolAward2023,
    }
    
	internal static class HotReloadSuggestionsHelper {
        internal static void SetSuggestionsShown(HotReloadSuggestionKind hotReloadSuggestionKind) {
            if (EditorPrefs.GetBool($"HotReloadWindow.SuggestionsShown.{hotReloadSuggestionKind}")) {
                return;
            }
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}", true);
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsShown.{hotReloadSuggestionKind}", true);
            AlertEntry entry;
            if (suggestionMap.TryGetValue(hotReloadSuggestionKind, out entry) && !HotReloadTimelineHelper.Suggestions.Contains(entry)) {
                HotReloadTimelineHelper.Suggestions.Insert(0, entry);
                HotReloadState.ShowingRedDot = true;
            }
        }
        
        internal static bool CheckSuggestionActive(HotReloadSuggestionKind hotReloadSuggestionKind) {
            return EditorPrefs.GetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}");
        }
        
        internal static void SetSuggestionInactive(HotReloadSuggestionKind hotReloadSuggestionKind) {
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}", false);
            AlertEntry entry;
            if (suggestionMap.TryGetValue(hotReloadSuggestionKind, out entry)) {
                HotReloadTimelineHelper.Suggestions.Remove(entry);
            }
        }
        
        internal static void InitSuggestions() {
            foreach (HotReloadSuggestionKind value in Enum.GetValues(typeof(HotReloadSuggestionKind))) {
                if (!CheckSuggestionActive(value)) {
                    continue;
                }
                AlertEntry entry;
                if (suggestionMap.TryGetValue(value, out entry) && !HotReloadTimelineHelper.Suggestions.Contains(entry)) {
                    HotReloadTimelineHelper.Suggestions.Insert(0, entry);
                }
            }
        }
        
        internal static HotReloadSuggestionKind? FindSuggestionKind(AlertEntry targetEntry) {
            foreach (KeyValuePair<HotReloadSuggestionKind, AlertEntry> pair in suggestionMap) {
                if (pair.Value.Equals(targetEntry)) {
                    return pair.Key;
                }
            }
            return null;
        }
        
        internal static readonly OpenURLButton recompileTroubleshootingButton = new OpenURLButton("Documentation", Constants.RecompileTroubleshootingURL);
        internal static readonly OpenURLButton featuresDocumentationButton = new OpenURLButton("Documentation", Constants.FeaturesDocumentationURL);
        public static Dictionary<HotReloadSuggestionKind, AlertEntry> suggestionMap = new Dictionary<HotReloadSuggestionKind, AlertEntry> {
            { HotReloadSuggestionKind.UnityBestDevelopmentToolAward2023, new AlertEntry(
                AlertType.Suggestion, 
                "Vote for the \"Best Development Tool\" Award!", 
                "Hot Reload was nominated for the \"Best Development Tool\" Award. Please consider voting. Thank you!",
                actionData: () => {
                    GUILayout.Space(6f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Vote ")) {
                            Application.OpenURL(Constants.VoteForAwardURL);
                            SetSuggestionInactive(HotReloadSuggestionKind.UnityBestDevelopmentToolAward2023);
                        }
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.UnsupportedChanges, new AlertEntry(
                AlertType.Suggestion, 
                "Which changes does Hot Reload support?", 
                "Hot Reload supports most code changes, but there are some limitations. Generally, changes to the method definition and body are allowed. Non-method changes (like adding/editing classes and fields) are not supported. See the documentation for the list of current features and our current roadmap",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        featuresDocumentationButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.UnsupportedPackages, new AlertEntry(
                AlertType.Suggestion, 
                "Unsupported package detected",
                "The following packages are only partially supported: ECS, Mirror, Fishnet, and Photon. Hot Reload will work in the project, but changes specific to those packages might not work. Contact us if these packages are a big part of your project",
                iconType: AlertType.UnsupportedChange,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        HotReloadAboutTab.contactButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.SymbolicLinks, new AlertEntry(
                AlertType.Suggestion, 
                "Symbolic links are not fully supported",
                "We’ve detected symbolically linked files in your project. Please note that changes to these files will be ignored. Contact us if symbolic links are a big part of your project",
                iconType: AlertType.UnsupportedChange,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        HotReloadAboutTab.discordButton.OnGUI();
                        GUILayout.Space(5f);
                        HotReloadAboutTab.contactButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
             )},
            { HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges, new AlertEntry(
                AlertType.Suggestion, 
                "Unity recompiles on enter/exit play mode?",
                "If you have an issue with the Unity Editor recompiling when the Play Mode state changes, please consult the documentation, and don’t hesitate to reach out to us if you need assistance",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        recompileTroubleshootingButton.OnGUI();
                        GUILayout.Space(5f);
                        HotReloadAboutTab.discordButton.OnGUI();
                        GUILayout.Space(5f);
                        HotReloadAboutTab.contactButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
        };
        
        static TaskCompletionSource<bool> symbolicLinkChecker { get; set; }
        static ListRequest listRequest;
        static string[] unsupportedPackages = new[] {
            "com.unity.entities",
            "com.firstgeargames.fishnet",
        };
        static List<string> unsupportedPackagesList;
        static DateTime lastPlaymodeChange;
        
        public static void Init() {
            listRequest = Client.List(offlineMode: false, includeIndirectDependencies: true);

            EditorApplication.playModeStateChanged += state => {
                lastPlaymodeChange = DateTime.UtcNow;
            };
            CompilationPipeline.compilationStarted += obj => {
                if (DateTime.UtcNow - lastPlaymodeChange < TimeSpan.FromSeconds(1)) {
                    SetSuggestionsShown(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges);
                }
            };
            StartCheckingSymlinks(Path.GetFullPath("Assets")).Forget();
            InitSuggestions();
        }

        public static void Check() {
            if (listRequest.IsCompleted && unsupportedPackagesList == null) {
                unsupportedPackagesList = new List<string>();
                var packages = listRequest.Result;
                foreach (var packageInfo in packages) {
                    if (unsupportedPackages.Contains(packageInfo.name)) {
                        unsupportedPackagesList.Add(packageInfo.name);
                    }
                }
                if (unsupportedPackagesList.Count > 0) {
                    SetSuggestionsShown(HotReloadSuggestionKind.UnsupportedPackages);
                }
            }
            if (symbolicLinkChecker.Task.IsCompleted && symbolicLinkChecker.Task.Result) {
                SetSuggestionsShown(HotReloadSuggestionKind.SymbolicLinks);
            }
        }
        
        public static async Task StartCheckingSymlinks(string assetsPath) {
            if (symbolicLinkChecker == null) {
                symbolicLinkChecker = new TaskCompletionSource<bool>();
            }
            if (symbolicLinkChecker.Task.IsCompleted) {
                return;
            }
            await Task.Run(() => {
                symbolicLinkChecker.TrySetResult(HasSymbolicLinks(assetsPath));
            });
        }

        static bool HasSymbolicLinks(string directoryPath) {
            try {
                // Check for symbolic links in the current directory.
                foreach (string filePath in Directory.GetFiles(directoryPath)) {
                    if (IsSymbolicLink(filePath)) {
                        return true;
                    }
                }

                // Recursively check subdirectories.
                foreach (string subdirectoryPath in Directory.GetDirectories(directoryPath)) {
                    var hasSymbolicLinks = HasSymbolicLinks(subdirectoryPath);
                    if (hasSymbolicLinks) {
                        return true;
                    }
                }
            } catch {
                // best effort
            }
            return false;
        }

        static bool IsSymbolicLink(string path) {
            try {
                FileAttributes attributes = File.GetAttributes(path);

                // Check if the ReparsePoint flag is set in the file attributes.
                return (attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
            } catch {
                return false; // Treat as non-symbolic link if there's an error.
            }
        }
	}

}
