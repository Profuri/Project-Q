using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace TinyGiantStudio.Text
{
    /// <summary>
    /// List of problems this is trying to solve
    ///     1. Sometimes, unity destroys the mesh of the text when destroying the other game object with the same mesh. (Shared mesh)
    ///         This checks if a mesh is empty, then updates it to fix empty texts
    ///     2. On prefab updates, the instance of the mesh in scene doesn't get updated automatically.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/text/text-updater")]
    public class TextUpdater : MonoBehaviour
    {
        private Modular3DText Text => GetComponent<Modular3DText>();

#if UNITY_EDITOR
        [SerializeField, HideInInspector] private int GUIID;
#endif

        /// <summary>
        /// Awake has been switched to on enable, because, domain reloads cleared the
        /// </summary>
        [ExecuteAlways]
        private void OnEnable()
        {
#if UNITY_EDITOR
            OnEnableEditorOnly();
#endif
            if (!Text)
                return;

            if (EmptyText(Text))
                Text.CleanUpdateText(); //This needs to be clean update.Otherwise, the text often gets wrong information passed from prefab that tells UpdateText() that only a few or no letters need to be created even though it is empty
        }

#if UNITY_EDITOR

        private void OnEnableEditorOnly()
        {
            if (!Application.isPlaying)
                this.enabled = true;

            // If a text with combined mesh is duplicated, they have the same mesh. If one of those
            // is deleted, like when updating the text on one of them, both are destroyed, that is,
            // the shared mesh is destroyed This is used to make a new copy

            #region Duplication Shared mesh Fix

            if (GUIID == 0)
            {
                GUIID = gameObject.GetInstanceID();
                EditorUtility.SetDirty(this);
            }
            else if (GUIID != gameObject.GetInstanceID())
            {
                GameObject old = EditorUtility.InstanceIDToObject(GUIID) as GameObject;
                if (old != null)
                {
                    MeshFilter original = old.GetComponent<MeshFilter>();
                    MeshFilter myMeshFilter = GetComponent<MeshFilter>();

                    if (myMeshFilter != null && original != null && myMeshFilter != original)
                    {
                        var mesh = myMeshFilter.sharedMesh;
                        if (mesh != null)
                        {
                            if (myMeshFilter.sharedMesh == original.sharedMesh)
                            {
                                myMeshFilter.sharedMesh = new Mesh()
                                {
                                    vertices = mesh.vertices,
                                    triangles = mesh.triangles,
                                    normals = mesh.normals,
                                    tangents = mesh.tangents,
                                    bounds = mesh.bounds,
                                    uv = mesh.uv
                                };
                            }
                        }
                    }
                }

                GUIID = gameObject.GetInstanceID();
                EditorUtility.SetDirty(this);
            }

            #endregion Duplication Shared mesh Fix

            if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
                PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdated; /// The problem with this is, this works until This gets lost during domain reloads
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                this.enabled = false;
                return;
            }
        }

        private void OnDestroy()
        {
            EditorApplication.delayCall -= () => CheckPrefab();
        }

        private void OnPrefabInstanceUpdated(GameObject instance)
        {
            EditorApplication.delayCall += () => CheckPrefab();
        }

        private void CheckPrefab()
        {
            if (!this)
                return;

            if (Text == null)
                return;

            bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
            if (prefabConnected)
            {
                EditorApplication.delayCall += () => Text.CleanUpdateText();
            }
        }

#endif

        private bool EmptyText(Modular3DText text)
        {
            if (string.IsNullOrEmpty(text.Text))
            {
                return false;
            }

            if (gameObject.GetComponent<MeshFilter>())
            {
                if (gameObject.GetComponent<MeshFilter>().sharedMesh != null)
                {
                    return false;
                }
            }

            if (text.characterObjectList.Count > 0)
            {
                for (int i = 0; i < text.characterObjectList.Count; i++)
                {
                    if (text.characterObjectList[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}