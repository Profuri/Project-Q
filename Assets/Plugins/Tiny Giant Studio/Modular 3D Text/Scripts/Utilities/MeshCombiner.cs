using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TinyGiantStudio.Layout;

namespace TinyGiantStudio.Text
{
    public static class MeshCombiner
    {
        public static List<Mesh> CombinedMesh(List<MeshLayout> meshLayouts, Vector3 scale, bool useIncreasedVerticiesCountForCombinedMesh)
        {
            if (meshLayouts == null)
                return null;

            List<List<MeshLayout>> meshFiltersUpperList = new List<List<MeshLayout>>();
            int listNumber = 0;
            List<MeshLayout> firstList = new List<MeshLayout>();
            meshFiltersUpperList.Add(firstList);

            bool tooMuchVerts = false;

            int verteciesCount = 0;
            for (int j = 0; j < meshLayouts.Count; j++)
            {
                if (meshLayouts[j].mesh)
                {
                    if (meshLayouts[j].mesh.isReadable)
                    {
                        if (useIncreasedVerticiesCountForCombinedMesh || verteciesCount + meshLayouts[j].mesh.vertices.Length < 65535)
                        {
                            verteciesCount += meshLayouts[j].mesh.vertices.Length;
                            meshFiltersUpperList[listNumber].Add(meshLayouts[j]);

                            if (verteciesCount + meshLayouts[j].mesh.vertices.Length >= 65535)
                                tooMuchVerts = true;
                        }
                        else
                        {
                            verteciesCount = 0;
                            List<MeshLayout> newList = new List<MeshLayout>();
                            meshFiltersUpperList.Add(newList);
                            listNumber++;
                            verteciesCount += meshLayouts[j].mesh.vertices.Length;
                            meshFiltersUpperList[listNumber].Add(meshLayouts[j]);
                        }
                    }
                    else
                    {
                        Debug.LogError(meshLayouts[j].mesh + " mesh isReadable is set to false. Read/Write must be enabled on import settings.");
                    }
                }
            }
            List<Mesh> combinedMeshes = new List<Mesh>();
            for (int k = 0; k < meshFiltersUpperList.Count; k++)
            {
                CombineInstance[] combine = new CombineInstance[meshFiltersUpperList[k].Count];

                int i = 0;
                while (i < meshFiltersUpperList[k].Count)
                {
                    combine[i].mesh = meshFiltersUpperList[k][i].mesh;

                    Matrix4x4 m = Matrix4x4.TRS(RemoveNaNAndInfinityErrorIfAny(meshFiltersUpperList[k][i].position), RemoveNaNAndInfinityErrorIfAny(meshFiltersUpperList[k][i].rotation), scale);
                    combine[i].transform = m;

                    i++;
                }


                List<CombineInstance> combinedList = new List<CombineInstance>();
                for (int j = 0; j < combine.Length; j++)
                {
                    if (combine[j].mesh != null)
                        combinedList.Add(combine[j]);
                }
                combine = combinedList.ToArray();

                Mesh finalMesh = new Mesh();

                if (useIncreasedVerticiesCountForCombinedMesh && tooMuchVerts)
                    finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                finalMesh.CombineMeshes(combine);

                combinedMeshes.Add(finalMesh);
            }
            return combinedMeshes;
        }


        public static Vector3 RemoveNaNAndInfinityErrorIfAny(Vector3 vector3)
        {
            if (float.IsNaN(vector3.x) || float.IsInfinity(vector3.x))
                vector3.x = 0;
            if (float.IsNaN(vector3.y) || float.IsInfinity(vector3.y))
                vector3.y = 0;
            if (float.IsNaN(vector3.z) || float.IsInfinity(vector3.z))
                vector3.z = 0;

            return vector3;
        }
        public static Quaternion RemoveNaNAndInfinityErrorIfAny(Quaternion quaternion)
        {
            if (float.IsNaN(quaternion.x) || float.IsInfinity(quaternion.x))
                quaternion.x = 0;
            if (float.IsNaN(quaternion.y) || float.IsInfinity(quaternion.y))
                quaternion.y = 0;
            if (float.IsNaN(quaternion.z) || float.IsInfinity(quaternion.z))
                quaternion.z = 0;

            return quaternion;
        }

#if UNITY_EDITOR
        //todo
        private static (bool, bool) CheckDirty(GameObject gameObject)
        {
            bool positionDirty = false;
            bool rotationDirty = false;

            if (!PrefabUtility.IsPartOfAnyPrefab(gameObject))
                return (positionDirty, rotationDirty);

            var modifications = PrefabUtility.GetPropertyModifications(gameObject);
            for (int i = 0; i < modifications.Length; i++)
            {
                if (modifications[i] != null)
                {
#if UNITY_2018_3_OR_NEWER
                    if (PrefabUtility.IsDefaultOverride(modifications[i])) continue;
#endif
                    if (modifications[i].propertyPath.Contains("m_LocalPosition"))
                        positionDirty = true;
                    if (modifications[i].propertyPath.Contains("m_LocalRotation"))
                        rotationDirty = true;
                }
            }
            return (positionDirty, rotationDirty);
        }
#endif
    }
}