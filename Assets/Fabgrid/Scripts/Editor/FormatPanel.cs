using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fabgrid;
using System;
using System.Linq;
using System.Reflection;
using PanelEditor;
using StageStructureConvertSystem;
using UnityEngine.UIElements;

namespace Fabgrid
{
    public class FormatPanel : Panel
    {
        private readonly Tilemap3D _tilemap;
        
        private Type _currentType;
        public Type CurrentType => _currentType;

        private VisualTreeAsset _toggleField;
        private VisualTreeAsset _dropDownField;
        private VisualTreeAsset _inputField;

        private Dictionary<FieldInfo, PanelField> _fieldDictionary;
        
        public FormatPanel(string name, string stylesheetPath, string visualTreeAssetPath, State state,
            string buttonIconPath, Tilemap3D tilemap) : base(name, stylesheetPath, visualTreeAssetPath, state,
            buttonIconPath)
        {
            Name = name;
            StylesheetPath = stylesheetPath;
            VisualTreeAssetPath = visualTreeAssetPath;
            State = state;
            ButtonIconPath = buttonIconPath;

            _tilemap = tilemap;

            LoadFieldInfo();

            
            _toggleField = Resources.Load<VisualTreeAsset>(VisualTreeAssetPath + "/Fields/ToggleField");
            _dropDownField = Resources.Load<VisualTreeAsset>(VisualTreeAssetPath + "/Fields/DropdownField");
            _inputField = Resources.Load<VisualTreeAsset>(VisualTreeAssetPath + "/Fields/InputField");
        }

        [ContextMenu("LoadFieldInfo")]
        private void LoadFieldInfo()
        {
            //After refactoring by interface;

            _fieldDictionary.Clear();
            
            Type type = _tilemap.selectedGameObject.GetType();
            if (_currentType == null || _currentType != type)
            {
                _currentType = type;
                
                var fieldInfoList = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

                foreach (var fieldInfo in fieldInfoList)
                {
                    if (Attribute.IsDefined(fieldInfo, typeof(SerializeField)))
                    {
                        Debug.Log($"FieldInfo: {fieldInfo}");
                        //var field = _factory.CreateUI(fieldInfo);
                    }
                }
            }
        }

        private void CreateVisualTree(FieldInfo info)
        {
            Type type = info.GetType();

            if (_fieldDictionary.ContainsKey(info)) return;
            if (type.IsEnum)
            {
                _fieldDictionary.Add(info, new ToggleField(_dropDownField,info));    
            }
            else if (type.IsValueType)
            {
                if (type == typeof(float))
                {
                    _fieldDictionary.Add(info, new InputField<float>(_inputField,info));
                }
                else if (type == typeof(bool))
                {
                    Debug.Log("IsBoolean");
                }
            }
        }
        
    }
}