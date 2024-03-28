using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fabgrid;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using PanelEditor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Fabgrid
{
    public class FormatPanel : Panel
    {
        private readonly Tilemap3D _tilemap;

        private Type _currentType;
        public Type CurrentType => _currentType;

        private VisualElement _root;


        private VisualTreeAsset _toggleField;
        private VisualTreeAsset _dropDownField;
        private VisualTreeAsset _inputField;

        private static readonly string s_removeString = "FormatPanel.uxml";
        private static readonly string[] s_uxmlNames = 
            { "Fields/ToggleField.uxml", "Fields/DropdownField.uxml", "Fields/InputField.uxml" };

        public void SetRoot(VisualElement root)
        {
            _root = root;
        }

        public FormatPanel(string name, string stylesheetPath, string visualTreeAssetPath, State state,
            string buttonIconPath, Tilemap3D tilemap, ref VisualElement root) : base(name, stylesheetPath, visualTreeAssetPath, state,
            buttonIconPath)
        {
            Name = name;
            StylesheetPath = stylesheetPath;
            VisualTreeAssetPath = visualTreeAssetPath;
            State = state;
            ButtonIconPath = buttonIconPath;

            _tilemap = tilemap;
            _root = root;


            string[] fieldPathArray = new string[s_uxmlNames.Length];



            StringBuilder sBuilder = new StringBuilder();

            int panelNameStartIdx = VisualTreeAssetPath.Length - s_removeString.Length;
            sBuilder.Append(VisualTreeAssetPath);
            sBuilder.Remove(panelNameStartIdx, s_removeString.Length);

            for (int i = 0; i < s_uxmlNames.Length; i++)
            {
                sBuilder.Append(s_uxmlNames[i]);
                fieldPathArray[i] = sBuilder.ToString();
                sBuilder.Remove(panelNameStartIdx,s_uxmlNames[i].Length);
            }

#if UNITY_EDITOR
            _toggleField = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(fieldPathArray[0]);
            _dropDownField = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(fieldPathArray[1]);
            _inputField = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(fieldPathArray[2]);
#endif


            _tilemap.OnSelectedPanelChanged += LoadFieldInfo;


            List<FieldInfo> fieldInfos = FieldInfoStorage.GetFieldInfo();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                CreateVisualTree(fieldInfo);
            }
        }

        ~FormatPanel()
        {
            GC.Collect();
        }

        public void LoadFieldInfo()
        {
            if (_tilemap.lastSelectedGameObject == null)
            {
                Debug.Log($"TileMapLastSelectedGameObject is null!");
                return;
            }
            
            _root?.Clear();
        
            if (_tilemap.lastSelectedGameObject.TryGetComponent(out IProvidableFieldInfo provideInfo))
            {
                List<FieldInfo> fieldInfos = provideInfo.GetFieldInfos();
                FieldInfoStorage.SetFieldInfo(provideInfo);
                
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    CreateVisualTree(fieldInfo);
                }
            }
        }

        private void CreateVisualTree(FieldInfo info)
        {
            Type type = info.FieldType;


            PanelField panelField = null;

            if (_root == null)
            {
                Debug.Log("Root is null");
                return;
            }
            
            if (type.IsEnum)
                panelField = new DropdownField(_root, _dropDownField, info);
            else if (type.IsValueType)
            {
                if (type == typeof(bool))
                    panelField = new ToggleField(_root, _toggleField, info);
                else if (type == typeof(float))
                    panelField = new InputField<float>(_root, _inputField, info);
                else if (type == typeof(int))
                    panelField = new InputField<int>(_root, _inputField, info);
            }
        }
    }
}