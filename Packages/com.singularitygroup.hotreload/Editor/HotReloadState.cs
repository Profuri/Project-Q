using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal static class HotReloadState {
        private const string LastPatchIdKey = "HotReloadWindow.LastPatchId";
        private const string ShowingRedDotKey = "HotReloadWindow.ShowingRedDot";
        
        public static string LastPatchId {
            get { return SessionState.GetString(LastPatchIdKey, string.Empty); }
            set { SessionState.SetString(LastPatchIdKey, value); }
        }
        
        public static bool ShowingRedDot {
            get { return SessionState.GetBool(ShowingRedDotKey, false); }
            set { SessionState.SetBool(ShowingRedDotKey, value); }
        }
    }

}
