using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fabgrid;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Codice.Client.Common.GameUI;
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

        private Dictionary<FieldInfo, PanelField> _fieldDictionary;

        public List<FieldInfo> GetFieldInfoList
        {
            get
            {
                Debug.Log("GetFieldInfoList");
                foreach (var kvp in _fieldDictionary)
                {
                    Debug.Log($"FieldInfo: {kvp.Key} PanelField: {kvp.Value}");
                }

                return _fieldDictionary.Keys.ToList();
            }
        }
        


        //This is test Code. Will be changed.
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


            _fieldDictionary = new Dictionary<FieldInfo, PanelField>();

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

            _toggleField = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(fieldPathArray[0]);
            _dropDownField = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(fieldPathArray[1]);
            _inputField = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(fieldPathArray[2]);    

            _tilemap.OnSelectedPanelChanged += LoadFieldInfo;


            List<FieldInfo> fieldInfos = FieldInfoStorage.GetFieldInfo();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                CreateVisualTree(fieldInfo);
            }
            
            Debug.LogError("CreateFormatPanel");
        }


        //selectedGameObject가 바뀌었을 때 이벤트 형식으로 구독하여 실행해주거나 다른 방식으로라도 구독형식으로 만들면 될 것 같음.
        // [ContextMenu("LoadFieldInfo")]
        //
        // public void LoadFieldInfo()
        // {
        //     if (_tilemap.lastSelectedGameObject == null)
        //     {
        //         Debug.Log($"TileMapLastSelectedGameObject is null!");
        //         return;
        //     }
        //     
        //     _fieldDictionary.Clear();
        //     _root?.Clear();
        //     
        //     if (_tilemap.selectedTile.prefab.TryGetComponent(out IProvidableFieldInfo provideInfo))
        //     {
        //         var fieldInfoList = new List<FieldInfo>();
        //     
        //         fieldInfoList = provideInfo.GetFieldInfos();
        //     
        //         foreach (var fieldInfo in fieldInfoList)
        //         {
        //             if (_fieldDictionary.ContainsKey(fieldInfo)) continue;
        //     
        //             CreateVisualTree(fieldInfo);
        //         }
        //     }
        // }
        public void LoadFieldInfo()
        {
            if (_tilemap.lastSelectedGameObject == null)
            {
                Debug.Log($"TileMapLastSelectedGameObject is null!");
                return;
            }
            
            _fieldDictionary.Clear();
            _root?.Clear();
        
            if (_tilemap.lastSelectedGameObject.TryGetComponent(out IProvidableFieldInfo provideInfo))
            {
                FieldInfoStorage.SetFieldInfo(provideInfo);
                
                
                List<FieldInfo> fieldInfos = FieldInfoStorage.GetFieldInfo();
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    CreateVisualTree(fieldInfo);
                }
                
                Debug.Log("SaveFieldInfo");
               // foreach (var fieldInfo in fieldInfoList)
               // {
               //     if (_fieldDictionary.ContainsKey(fieldInfo)) continue;
        
               //     CreateVisualTree(fieldInfo);
               // }
            }
        }

        private void CreateVisualTree(FieldInfo info)
        {
            Type type = info.FieldType;

            if (_fieldDictionary.ContainsKey(info)) return;                                                                      

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
            _fieldDictionary.Add(info,panelField);
        }
        

    }
}