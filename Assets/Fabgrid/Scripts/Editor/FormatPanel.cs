using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fabgrid;
using System;
using System.Linq;
using System.Reflection;
using PanelEditor;
using StageStructureConvertSystem;
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

        private Dictionary<FieldInfo, PanelField> _fieldDictionary;
        
        public FormatPanel(string name, string stylesheetPath, string visualTreeAssetPath, State state,
            string buttonIconPath, Tilemap3D tilemap,VisualElement root) : base(name, stylesheetPath, visualTreeAssetPath, state,
            buttonIconPath)
        {
            Name = name;
            StylesheetPath = stylesheetPath;
            VisualTreeAssetPath = visualTreeAssetPath;
            State = state;
            ButtonIconPath = buttonIconPath;

            _tilemap = tilemap;


            _fieldDictionary = new Dictionary<FieldInfo, PanelField>();
            
            _toggleField   =    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreeAssetPath + "/Fields/ToggleField");
            _dropDownField =    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreeAssetPath + "/Fields/DropdownField");
            _inputField    =    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VisualTreeAssetPath + "/Fields/InputField");

            _tilemap.OnSelectedObjectChanged += LoadFieldInfo;
        }
        
        //selectedGameObject가 바뀌었을 때 이벤트 형식으로 구독하여 실행해주거나 다른 방식으로라도 구독형식으로 만들면 될 것 같음.
        [ContextMenu("LoadFieldInfo")]
        private void LoadFieldInfo()
        {
            //After refactoring by interface;
            _fieldDictionary.Clear();

            _root.Clear();
            Type type = _tilemap.selectedGameObject.GetType();
            if (_currentType == null || _currentType != type)
            {
                _currentType = type;
                
                var fieldInfoList = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

                foreach (var fieldInfo in fieldInfoList)
                {
                    if (Attribute.IsDefined(fieldInfo, typeof(SerializeField)))
                    {
                        CreateVisualTree(fieldInfo);
                        Debug.Log($"FieldInfo: {fieldInfo}");
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
                _fieldDictionary.Add(info, new ToggleField(_root,_dropDownField,info));    
            }
            else if (type.IsValueType)
            {
                if (type == typeof(bool))
                {
                    _fieldDictionary.Add(info,new ToggleField(_root,_inputField,info));
                    Debug.Log("IsBoolean");
                }
                else if (type == typeof(float))
                {
                    _fieldDictionary.Add(info, new InputField<float>(_root,_inputField,info));
                }
                else if (type == typeof(int))
                {
                    _fieldDictionary.Add(info, new InputField<int>(_root,_inputField,info));
                }
            }
        }
    }
}