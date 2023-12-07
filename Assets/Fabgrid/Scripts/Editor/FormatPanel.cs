using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fabgrid;
using System;
using System.Reflection;
//using StageStructureConvertSystem;

namespace Fabgrid
{
    public class FormatPanel : Panel
    {
        private readonly Tilemap3D _tilemap;

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

            _fieldInfoList = new List<FieldInfo>();

            LoadFieldInfo();

        }

        public List<FieldInfo> _fieldInfoList;

        private void LoadFieldInfo()
        {
            _fieldInfoList.Clear();

            //Type type = typeof(StructureObjectUnitBase);
            //foreach (FieldInfo fieldInfo in type.GetFields())
            //{
            //    Debug.Log(fieldInfo);
            //}
        }
    }
}