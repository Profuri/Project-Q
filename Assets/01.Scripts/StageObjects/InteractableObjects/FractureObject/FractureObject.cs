using System.Collections.Generic;
using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;
using Unity.VisualScripting;

public class FractureObject : InteractableObject
{
    private bool _edgeSet = false;
    private Vector3 _edgeVertex = Vector3.zero;
    private Vector2 _edgeUV = Vector2.zero;
    private Plane _edgePlane = new Plane();

    [SerializeField] private int _cutCascades = 1;
    [SerializeField] private float _explodeForce = 0;

    private MeshFilter _meshFilter;
    public MeshFilter MeshFilter => _meshFilter;

    private MeshRenderer _meshRenderer;
    public MeshRenderer MeshRenderer => _meshRenderer;

    
    public override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        var originalMesh = _meshFilter.mesh;
        originalMesh.RecalculateBounds();
        var parts = new List<FracturePart>();
        var subParts = new List<FracturePart>();

        var mainPart = SceneControlManager.Instance.AddObject("FracturePart") as FracturePart;
        mainPart.UV = originalMesh.uv;
        mainPart.Vertices = originalMesh.vertices;
        mainPart.Normal = originalMesh.normals;
        mainPart.Triangle = new int[originalMesh.subMeshCount][];
        mainPart.Bounds = originalMesh.bounds;

        for (var i = 0; i < originalMesh.subMeshCount; i++)
        {
            mainPart.Triangle[i] = originalMesh.GetTriangles(i);
        }

        parts.Add(mainPart);

        for (var c = 0; c < _cutCascades; c++)
        {
            foreach (var part in parts)
            {
                var bounds = part.Bounds;
                bounds.Expand(0.5f);

                var plane = new Plane(
                    Random.onUnitSphere,
                    new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y),
                        Random.Range(bounds.min.z, bounds.max.z))
                );

                subParts.Add(GenerateMesh(part, plane, true));
                subParts.Add(GenerateMesh(part, plane, false));
                
                SceneControlManager.Instance.DeleteObject(part);
            }
            
            parts = new List<FracturePart>(subParts);
            subParts.Clear();
        }

        foreach (var part in parts)
        {
            part.Setting(this);
            part.AddForce(part.Bounds.center * _explodeForce, transform.position);
        }


        //SceneControlManager.Instance.DeleteObject(this);
        Activate(false);
    }

    private FracturePart GenerateMesh(FracturePart original, Plane planeUnit, bool left)
    {
        var partMesh = SceneControlManager.Instance.AddObject("FracturePart") as FracturePart;
        var ray1 = new Ray();
        var ray2 = new Ray();
        
        for (var i = 0; i < original.Triangle.Length; i++)
        {
            var triangles = original.Triangle[i];
            _edgeSet = false;

            for (var j = 0; j < triangles.Length; j += 3)
            {
                var sideA = planeUnit.GetSide(original.Vertices[triangles[j]]) == left;
                var sideB = planeUnit.GetSide(original.Vertices[triangles[j + 1]]) == left;
                var sideC = planeUnit.GetSide(original.Vertices[triangles[j + 2]]) == left;

                var sideCount = (sideA ? 1 : 0) + (sideB ? 1 : 0) + (sideC ? 1 : 0);

                if (sideCount == 0)
                {
                    continue;
                }
                
                if (sideCount == 3)
                {
                    partMesh.AddTriangle(
                        i,
                        original.Vertices[triangles[j]], original.Vertices[triangles[j + 1]], original.Vertices[triangles[j + 2]],
                        original.Normal[triangles[j]], original.Normal[triangles[j + 1]], original.Normal[triangles[j + 2]],
                        original.UV[triangles[j]], original.UV[triangles[j + 1]], original.UV[triangles[j + 2]]
                    );
                    continue;
                }

                //cut points
                var singleIndex = sideB == sideC ? 0 : sideA == sideC ? 1 : 2;

                ray1.origin = original.Vertices[triangles[j + singleIndex]];
                var dir1 = original.Vertices[triangles[j + ((singleIndex + 1) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                ray1.direction = dir1;
                planeUnit.Raycast(ray1, out var enter1);
                var lerp1 = enter1 / dir1.magnitude;

                ray2.origin = original.Vertices[triangles[j + singleIndex]];
                var dir2 = original.Vertices[triangles[j + ((singleIndex + 2) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                ray2.direction = dir2;
                planeUnit.Raycast(ray2, out var enter2);
                var lerp2 = enter2 / dir2.magnitude;

                //first vertex = Anchor
                AddEdge
                (
                    i,
                    partMesh,
                    left ? planeUnit.normal * -1f : planeUnit.normal,
                    ray1.origin + ray1.direction.normalized * enter1,
                    ray2.origin + ray2.direction.normalized * enter2,
                    Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                    Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2)
                );

                switch (sideCount)
                {
                    case 1:
                        partMesh.AddTriangle
                        (
                            i,
                            original.Vertices[triangles[j + singleIndex]],
                            ray1.origin + ray1.direction.normalized * enter1,
                            ray2.origin + ray2.direction.normalized * enter2,
                            original.Normal[triangles[j + singleIndex]],
                            Vector3.Lerp(original.Normal[triangles[j + singleIndex]], original.Normal[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            Vector3.Lerp(original.Normal[triangles[j + singleIndex]], original.Normal[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                            original.UV[triangles[j + singleIndex]],
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2)
                        );
                        continue;
                    case 2:
                        partMesh.AddTriangle
                        (
                            i,
                            ray1.origin + ray1.direction.normalized * enter1,
                            original.Vertices[triangles[j + ((singleIndex + 1) % 3)]],
                            original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector3.Lerp(original.Normal[triangles[j + singleIndex]], original.Normal[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.Normal[triangles[j + ((singleIndex + 1) % 3)]],
                            original.Normal[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.UV[triangles[j + ((singleIndex + 1) % 3)]],
                            original.UV[triangles[j + ((singleIndex + 2) % 3)]]
                        );
                        partMesh.AddTriangle
                        (
                            i,
                            ray1.origin + ray1.direction.normalized * enter1,
                            original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                            ray2.origin + ray2.direction.normalized * enter2,
                            Vector3.Lerp(original.Normal[triangles[j + singleIndex]], original.Normal[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.Normal[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector3.Lerp(original.Normal[triangles[j + singleIndex]], original.Normal[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.UV[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2)
                        );
                        continue;
                }
            }
        }

        partMesh.FillArrays();

        return partMesh;
    }

    private void AddEdge(int subMesh, FracturePart fracturePart, Vector3 normal, Vector3 vertex1, Vector3 vertex2, Vector2 uv1, Vector2 uv2)
    {
        if (!_edgeSet)
        {
            _edgeSet = true;
            _edgeVertex = vertex1;
            _edgeUV = uv1;
        }
        else
        {
            _edgePlane.Set3Points(_edgeVertex, vertex1, vertex2);

            fracturePart.AddTriangle(
                subMesh,
                _edgeVertex,
                _edgePlane.GetSide(_edgeVertex + normal) ? vertex1 : vertex2,
                _edgePlane.GetSide(_edgeVertex + normal) ? vertex2 : vertex1,
                normal,
                normal,
                normal,
                _edgeUV,
                uv1,
                uv2
            );
        }
    }
}