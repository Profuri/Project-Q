using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.Text
{
    /// <summary>
    /// Under construction. This will handle all the destroy object calls for modular 3d text in the future
    /// To-do: destroy calls on OnDestroy should be re factored to use this class
    /// </summary>
    public class CharacterCleanUp
    {
        private readonly Modular3DText text;

        public CharacterCleanUp(Modular3DText myText)
        {
            text = myText;
        }

        /// <summary>
        /// This destroys child objects for characters that are replaced by new ones or removed from text.
        /// </summary>
        /// <param name="startingFrom"></param>
        internal void DeleteReplacedChars(int startingFrom)
        {
            int toDeleteCount = text.characterObjectList.Count - startingFrom;

            text.Logger.LogToDeleteCharacters(toDeleteCount);

            List<GameObject> olderCharactersToDelete = GetOlderCharactersToDelete(startingFrom);

            foreach (GameObject child in olderCharactersToDelete)
            {
                text.Logger.LogDeletedCharacters(child.name);

                DestroyObject(child);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Once in a while, due to domain reloads while the delay calls to create 3D meshes is
        /// running, It gets called again and the older call to create characters still create them.
        /// Resulting in multiple objects for the same character. This is to clean them up
        /// </summary>
        internal void CleanupChildObjectsThatNoLongerExistsInCurrentText()
        {
            for (int i = text._allCharacterObjectList.Count - 1; i >= 0; i--)
            {
                if (text._allCharacterObjectList[i] == null)
                {
                    text._allCharacterObjectList.Remove(text._allCharacterObjectList[i]);
                    continue;
                }

                //to-do:Shouldn't this be opposite
                if (!text.characterObjectList.Contains(text._allCharacterObjectList[i]))
                {
                    GameObject obj = text._allCharacterObjectList[i];
                    EditorApplication.delayCall += () =>
                    {
                        CleanUpDelete(obj);
                    };
                }
            }

            if (!text.hideLettersInHierarchyInEditMode && !Application.isPlaying)
            {
                try
                {
                    foreach (Transform child in text.transform)
                    {
                        child.hideFlags = HideFlags.None;
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// This will do a blanket removal of all child objects that aren't referenced in Text.
        /// </summary>
        internal void CleanUpChildObjectsThatArentCharacterObject()
        {
            if (Application.isPlaying)
                return;

            List<GameObject> childs = new List<GameObject>();
            foreach (Transform child in text.transform)
                childs.Add(child.gameObject);

            foreach (GameObject child in childs)
            {
                if (!text.characterObjectList.Contains(child))
                    DestroyObject(child);
            }
        }

#endif

#if UNITY_EDITOR

        private void CleanUpDelete(GameObject obj)
        {
            try { Object.DestroyImmediate(obj); }
            catch { }
        }

#endif

        private List<GameObject> GetOlderCharactersToDelete(int startingFrom)
        {
            List<GameObject> toDelete = new List<GameObject>();
            for (int i = startingFrom; i < text.characterObjectList.Count; i++)
            {
                if (text.characterObjectList[i] != null)
                    toDelete.Add(text.characterObjectList[i]);
            }
            return toDelete;
        }

        private void DestroyObject(GameObject obj)
        {
            if (obj == null) return;

            if (text.characterObjectList.Contains(obj))
                text.characterObjectList.Remove(obj);

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                RunTimeDestroy(obj);
            }
            else
            {
                if (!PrefabUtility.IsPartOfAnyPrefab(obj))
                    Undo.DestroyObjectImmediate(obj); //if this causes error, please contact support and undo comment from the code below
                else
                    obj.SetActive(false);
            }
#else
            RunTimeDestroy(obj);
#endif
        }

        private void RunTimeDestroy(GameObject obj)
        {
            if (text.gameObject.activeInHierarchy)
                DestroyObjectRuntime(obj);
            else
                RunTimeDestroyObjectOnDisabledText(obj);
        }

        private void DestroyObjectRuntime(GameObject obj)
        {
            obj.transform.SetParent(null);

            if (obj.name != "Space" && text.useModules)
            {
                float delay = GetDestroyDelay();

                obj.GetComponent<MonoBehaviour>().StopAllCoroutines();

                for (int i = 0; i < text.deletingModules.Count; i++)
                {
                    if (text.deletingModules[i].module)
                    {
                        if (text.startDeletingModuleFromChar)
                            obj.GetComponent<MonoBehaviour>().StartCoroutine(text.deletingModules[i].module.ModuleRoutine(obj, text.deletingModules[i].variableHolders));
                        else
                            text.StartCoroutine(text.deletingModules[i].module.ModuleRoutine(obj, text.deletingModules[i].variableHolders));
                    }
                }

                if (obj.GetComponent<MeshFilter>() != null)
                {
                    if (obj.GetComponent<MeshFilter>().sharedMesh != null)
                    {
                        if (text.generatedMeshes.Contains(obj.GetComponent<MeshFilter>().sharedMesh))
                        {
                            Object.Destroy(obj.GetComponent<MeshFilter>().sharedMesh, delay);
                            text.generatedMeshes.Remove(obj.GetComponent<MeshFilter>().sharedMesh);
                        }
                    }
                }
                Object.Destroy(obj, delay);
            }
            else
            {
                if (text.combineMeshDuringRuntime)
                {
                    if (obj.GetComponent<MeshFilter>() != null)
                        if (obj.GetComponent<MeshFilter>().sharedMesh != null)
                            Object.Destroy(obj.GetComponent<MeshFilter>().sharedMesh);
                }
                else
                {
                    if (obj.GetComponent<MeshFilter>() != null)
                    {
                        if (obj.GetComponent<MeshFilter>().sharedMesh != null)
                        {
                            if (text.generatedMeshes.Contains(obj.GetComponent<MeshFilter>().sharedMesh))
                            {
                                Object.Destroy(obj.GetComponent<MeshFilter>().sharedMesh);
                                text.generatedMeshes.Remove(obj.GetComponent<MeshFilter>().sharedMesh);
                            }
                        }
                    }
                }

                Object.Destroy(obj);
            }
        }

        private float GetDestroyDelay()
        {
            return !text.customDeleteAfterDuration && text.deletingModules.Count > 0 ? text.GetDeleteDurationFromEffects() : text.customDeleteAfterDuration ? text.deleteAfter : 0;
        }

        private void RunTimeDestroyObjectOnDisabledText(GameObject obj) => Object.Destroy(obj);
    }
}