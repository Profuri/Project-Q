#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Fabgrid
{
    public class SelectionBox : Tool
    {
        private bool isDragging = false;
        private Vector2 mouseDownPosition;
        private Rect selectionBoxRect;
        private Material selectMaterial;
        List<GameObject> selections = new List<GameObject>();
        private readonly GUIStyle boxStyle;
        private Texture2D boxBackground;

        public SelectionBox(Tilemap3D tilemap) : base(tilemap)
        {
            toolType = ToolType.RectangleTool;
            LoadResources();

            boxStyle = new GUIStyle();
            boxStyle.normal.background = boxBackground;
            boxStyle.border.left = 1;
            boxStyle.border.right = 1;
            boxStyle.border.top = 1;
            boxStyle.border.bottom = 1;

            selections.Clear();
        }

        private void LoadResources()
        {
            var fabgridFolder = PathUtility.GetFabgridFolder();
            selectMaterial = AssetDatabase.LoadAssetAtPath<Material>($"{fabgridFolder}/Materials/FabgridSelectMaterial.mat");
            boxBackground = AssetDatabase.LoadAssetAtPath<Texture2D>($"{fabgridFolder}/Textures/SelectionBox.png");
        }

        public override void OnMouseDown(Event e)
        {
            if (e.button != 0) return;

            Reset();

            mouseDownPosition = e.mousePosition;
            isDragging = true;

            e.Use();
        }

        private void Reset()
        {
            isDragging = false;
            selectionBoxRect = new Rect(0f, 0f, 0f, 0f);
            selections.Clear();
        }

        public override void OnMouseUp(Event e)
        {
            if (e.button != 0) return;

            isDragging = false;
            selectionBoxRect = new Rect(0f, 0f, 0f, 0f);
            //selections.Clear();
        }

        private void Select(GameObject gameObject)
        {
            if (selections.Contains(gameObject))
                return;

            if (tilemap.gameObject == gameObject)
                return;

            if (gameObject.GetComponentInParent<Tilemap3D>() == null)
                return;

            selections.Add(gameObject);
        }

        public override void OnMouseDrag(Event e)
        {
            if (!isDragging) return;
            if (e.button != 0) return;

            var min = Vector2.Min(mouseDownPosition, e.mousePosition);
            var max = Vector2.Max(mouseDownPosition, e.mousePosition);

            selectionBoxRect = Rect.MinMaxRect(
                min.x,
                min.y,
                max.x,
                max.y);

            selections.Clear();

            foreach (var gameObject in HandleUtility.PickRectObjects(selectionBoxRect, true))
            {
                Select(gameObject);
            }
        }

        public override void OnRender(Event e)
        {
            if (isDragging)
            {
                RenderSelectionBox();
            }

            foreach(var selection in selections)
            { 
                foreach(var rend in selection.GetComponentsInChildren<MeshRenderer>())
                {
                    var filter = rend.GetComponent<MeshFilter>();

                    if (filter == null)
                        continue;

                    selectMaterial.SetPass(0);
                    var m = filter.gameObject.transform.localToWorldMatrix * Matrix4x4.Scale(Vector3.one * 1.1f);
                    Graphics.DrawMeshNow(filter.sharedMesh, m);
                }

            }
        }

        public override void OnDestroy()
        {
            selections.Clear();
        }

        public override void OnKeyDown(Event e)
        {
            if (e.keyCode == KeyCode.Delete)
            {
                foreach (var selection in selections)
                    Undo.DestroyObjectImmediate(selection.gameObject);

                selections.Clear();

                e.Use();
            }
        }

        private void RenderSelectionBox()
        {
            Handles.BeginGUI();

            GUI.backgroundColor = new Color(1f, 1f, 1f, 0.35f);
            GUI.Box(selectionBoxRect, "", boxStyle);

            Handles.EndGUI();
        }
    }
}

#endif