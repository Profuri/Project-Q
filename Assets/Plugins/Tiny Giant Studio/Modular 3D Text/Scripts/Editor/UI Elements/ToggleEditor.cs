using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TinyGiantStudio.Text.EditorFiles;

namespace TinyGiantStudio.Text
{
    [CustomEditor(typeof(Toggle))]
    public class ToggleEditor : Editor
    {
        private Toggle toggle;
        [SerializeField] private VisualTreeAsset visualTreeAsset;

        private GroupBox rootHolder;

        private EditorThemeManager _themeManager;

        private EditorThemeManager ThemeManager
        {
            get
            {
                if (_themeManager == null)
                    SetupThemeManager();

                return _themeManager;
            }
        }

        private void SetupThemeManager()
        {
            _themeManager = new EditorThemeManager(rootHolder);
        }

        public override VisualElement CreateInspectorGUI()
        {
            toggle = (Toggle)target;

            VisualElement root = new VisualElement();
            visualTreeAsset.CloneTree(root);

            rootHolder = root.Q<GroupBox>("RootHolder");

            var isOnToggle = root.Q<UnityEngine.UIElements.Toggle>("IsOnToggle");
            isOnToggle.RegisterValueChangedCallback(ev =>
            {
                toggle.VisualUpdate();
            });

            var graphicsOnObjectField = root.Q<ObjectField>("GraphicsOnObjectField");
            graphicsOnObjectField.RegisterValueChangedCallback(ev =>
            {
                toggle.VisualUpdate();
            });

            var GraphicsOffObjectField = root.Q<ObjectField>("GraphicsOffObjectField");
            GraphicsOffObjectField.RegisterValueChangedCallback(ev =>
            {
                toggle.VisualUpdate();
            });

            ThemeManager.Start();

            return root;
        }
    }
}