using System.Collections.Generic;
using UnityEngine;

public class FracturePartMesh : PoolableMono
{
    private readonly List<Vector3> _verticies = new List<Vector3>();
    private readonly List<Vector3> _normals = new List<Vector3>();
    private readonly List<List<int>> _triangles = new List<List<int>>();
    private readonly List<Vector2> _uvs = new List<Vector2>();

    public Vector3[] Vertices;
    public Vector3[] Normal;
    public int[][] Triangle;
    public Vector2[] UV;
    public Bounds Bounds;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void AddForce(Vector3 force, Vector3 position)
    {
        _rigidbody.AddForceAtPosition(force, position);
    }

    public void Setting(FractureObject origin)
    {
        var originalTransform = origin.transform;
        transform.position = originalTransform.position;
        transform.rotation = originalTransform.rotation;
        transform.localScale = originalTransform.localScale;

        var mesh = new Mesh
        {
            name = origin.MeshFilter.mesh.name,
            vertices = Vertices,
            normals = Normal,
            uv = UV
        };

        for (var i = 0; i < Triangle.Length; i++)
        {
            mesh.SetTriangles(Triangle[i], i, true);
        }
        Bounds = mesh.bounds;
        
        _meshRenderer.materials = origin.MeshRenderer.materials;
        _meshFilter.mesh = mesh;
        var meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
    }

    public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        if (_triangles.Count - 1 < submesh)
            _triangles.Add(new List<int>());

        _triangles[submesh].Add(_verticies.Count);
        _verticies.Add(vert1);
        
        _triangles[submesh].Add(_verticies.Count);
        _verticies.Add(vert2);
        
        _triangles[submesh].Add(_verticies.Count);
        _verticies.Add(vert3);
        
        _normals.Add(normal1);
        _normals.Add(normal2);
        _normals.Add(normal3);
        
        _uvs.Add(uv1);
        _uvs.Add(uv2);
        _uvs.Add(uv3);

        Bounds.min = Vector3.Min(Bounds.min, vert1);
        Bounds.min = Vector3.Min(Bounds.min, vert2);
        Bounds.min = Vector3.Min(Bounds.min, vert3);
        Bounds.max = Vector3.Min(Bounds.max, vert1);
        Bounds.max = Vector3.Min(Bounds.max, vert2);
        Bounds.max = Vector3.Min(Bounds.max, vert3);
    }

    public void FillArrays()
    {
        Vertices = _verticies.ToArray();
        Normal = _normals.ToArray();
        UV = _uvs.ToArray();
        Triangle = new int[_triangles.Count][];
        for (var i = 0; i < _triangles.Count; i++)
            Triangle[i] = _triangles[i].ToArray();
    }

    public override void Init()
    {
    }
}
