using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TinyGiantStudio.Text.EditorFiles
{
    /// <summary>
    /// This handles using the Material Preview Field Template.
    /// Call the Hook method
    /// </summary>
    public class MaterialPreviewManager
    {
        public void Hook(TemplateContainer container, SerializedProperty serializedProperty, string label, Material material)
        {
            if (container == null)
            {
                Debug.Log("No container");
                return;
            }

            UpdatePreivew(container, material);

            Label materialTypeLabel = container.Q<Label>("MaterialTypeLabel");
            materialTypeLabel.text = label;

            ObjectField objectField = container.Q<ObjectField>("MaterialField");
            objectField.BindProperty(serializedProperty);
            objectField.RegisterValueChangedCallback(e =>
            {
                UpdatePreivew(container, (Material)e.newValue);
            });
        }

        private void UpdatePreivew(TemplateContainer container, Material material)
        {
            VisualElement previewTexture = container.Q<VisualElement>("MaterialPreview");
            if (material != null)
            {
                Texture2D texture2D = AssetPreview.GetAssetPreview(material);
                if (texture2D != null)
                    previewTexture.style.backgroundImage = texture2D;
                else if (AssetPreview.IsLoadingAssetPreviews())
                    EditorApplication.delayCall += () => { WaitForLoadingToFinish(0, previewTexture, material); };
            }
            else
            {
                previewTexture.style.backgroundImage = null;
            }
        }

        /// <summary>
        /// to-do: The alternative to using this is editor coroutines but which requires additional unity package. Test that out.
        /// Note: On testing, it never took more than once to be called. So, the delayCall from UpdatePreview was always enough
        /// </summary>
        /// <param name="count"></param>
        /// <param name="previewTexture"></param>
        /// <param name="texture2D"></param>
        private void WaitForLoadingToFinish(int count, VisualElement previewTexture, Material material)
        {
            Texture2D texture2D = AssetPreview.GetAssetPreview(material);
            if (texture2D != null)
            {
                previewTexture.style.backgroundImage = texture2D;
            }
            else if (AssetPreview.IsLoadingAssetPreviews())
            {
                count++;
                if (count < 10)
                    EditorApplication.delayCall += () => { WaitForLoadingToFinish(count, previewTexture, material); };
            }
        }
    }
}