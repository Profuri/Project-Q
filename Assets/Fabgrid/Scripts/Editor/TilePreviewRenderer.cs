using UnityEngine;

namespace Fabgrid
{
    public class TilePreviewRenderer
    {
        private Tilemap3D tilemap;

        public TilePreviewRenderer(Tilemap3D tilemap)
        {
            this.tilemap = tilemap;
        }

        public void Render(Vector3 position)
        {
            if (tilemap.selectedTile?.prefab == null) return;
            if (tilemap.tilePreviewMesh == null) return;

            //tilemap.tilePreviewMaterial.SetPass(0);
            //Graphics.DrawMeshNow(tilemap.tilePreviewMesh,
            //    position,
            //    tilemap.tileRotation);

            var prefab = tilemap.selectedTile.prefab;

            var renderers = prefab.GetComponentsInChildren<MeshRenderer>();

            foreach (var rend in renderers)
            {
                var filter = rend.GetComponent<MeshFilter>();

                if (filter?.sharedMesh == null)
                    return;

                var previewMesh = CreatePreviewMesh(filter);

                rend.sharedMaterial.SetPass(0);
                Graphics.DrawMeshNow(previewMesh, position, tilemap.tileRotation);
            }
        }

        private static Mesh CreatePreviewMesh(MeshFilter meshFilter)
        {
            var combineInstances = new CombineInstance[1];

            combineInstances[0].mesh = meshFilter.sharedMesh;
            combineInstances[0].transform = meshFilter.transform.localToWorldMatrix;

            var previewMesh = new Mesh();
            previewMesh.CombineMeshes(combineInstances);

            return previewMesh;
        }
    }
}