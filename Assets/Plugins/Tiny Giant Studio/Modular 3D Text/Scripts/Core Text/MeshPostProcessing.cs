using UnityEngine;

namespace TinyGiantStudio.Text
{
    public class MeshPostProcessing : MonoBehaviour
    {
        public static Mesh PostProcess(Mesh mesh, MeshPostProcess meshPostProcess)
        {
            switch (meshPostProcess)
            {
                case MeshPostProcess.projectUV: //because it is the default one
                    return mesh;
                case MeshPostProcess.wrapUV:
                    return WrapUV(mesh);
                default:
                    break;
            }

            return mesh;
        }


        public static Mesh WrapUV(Mesh mesh)
        {
            if (!Application.isPlaying)
                mesh = Instantiate(mesh);

            Vector3[] vertexs = mesh.vertices;
            int[] tris = mesh.triangles;
            Vector2[] uvs = new Vector2[vertexs.Length];


            int i;
            for (i = 0; i < tris.Length; i += 3)
            {

                Vector3 a = vertexs[tris[i]];
                Vector3 b = vertexs[tris[i + 1]];
                Vector3 c = vertexs[tris[i + 2]];
                Vector3 side1 = b - a;
                Vector3 side2 = c - a;
                Vector3 N = Vector3.Cross(side1, side2);

                N = new Vector3(Mathf.Abs(N.normalized.x), Mathf.Abs(N.normalized.y), Mathf.Abs(N.normalized.z));



                if (N.x > N.y && N.x > N.z)
                {
                    uvs[tris[i]] = new Vector2(vertexs[tris[i]].z, vertexs[tris[i]].y);
                    uvs[tris[i + 1]] = new Vector2(vertexs[tris[i + 1]].z, vertexs[tris[i + 1]].y);
                    uvs[tris[i + 2]] = new Vector2(vertexs[tris[i + 2]].z, vertexs[tris[i + 2]].y);
                }
                else if (N.y > N.x && N.y > N.z)
                {
                    uvs[tris[i]] = new Vector2(vertexs[tris[i]].x, vertexs[tris[i]].z);
                    uvs[tris[i + 1]] = new Vector2(vertexs[tris[i + 1]].x, vertexs[tris[i + 1]].z);
                    uvs[tris[i + 2]] = new Vector2(vertexs[tris[i + 2]].x, vertexs[tris[i + 2]].z);
                }
                else if (N.z > N.x && N.z > N.y)
                {
                    uvs[tris[i]] = new Vector2(vertexs[tris[i]].x, vertexs[tris[i]].y);
                    uvs[tris[i + 1]] = new Vector2(vertexs[tris[i + 1]].x, vertexs[tris[i + 1]].y);
                    uvs[tris[i + 2]] = new Vector2(vertexs[tris[i + 2]].x, vertexs[tris[i + 2]].y);
                }

            }

            mesh.uv = uvs;
            mesh.RecalculateTangents();
            return mesh;
        }
    }





















    public enum MeshPostProcess
    {
        projectUV,
        wrapUV
    }
}