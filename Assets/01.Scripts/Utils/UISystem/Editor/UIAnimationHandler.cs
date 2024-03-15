using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class UIAnimationHandler
{
    private readonly UIAnimator _animator;
    private readonly UIComponentEditor _componentEditor;
    private readonly UIAnimation _clip;
    private readonly List<FieldInfo> _fields;
    private bool _foldValue;
    
    public UIAnimationHandler(UIAnimator animator, UIAnimation clip, UIComponentEditor componentEditor)
    {
        _animator = animator;
        _clip = clip;
        _componentEditor = componentEditor;
        _fields = _clip.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
    }
    
    public void DrawClipGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                _foldValue = EditorGUILayout.Foldout(_foldValue, _clip.GetType().Name);
                GUILayout.Space(70);
                if (GUILayout.Button("Remove"))
                {
                    RemoveBtnHandler();
                }
            }
            EditorGUILayout.EndHorizontal();

            if(_foldValue)
            {
                GUILayout.Space(10);
                foreach (var field in _fields)
                {
                    VariableLoad(field);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void RemoveBtnHandler()
    {
        _componentEditor.RemoveClip(_animator, _clip);
    }

    private void VariableLoad(FieldInfo field)
    {
        var value = field.GetValue(_clip);
        var origin = value;
        
        if (field.FieldType == typeof(int))
        {
            value = EditorGUILayout.IntField(field.Name, (int)value);
        } 
        else if (field.FieldType == typeof(float))
        {
            value = EditorGUILayout.FloatField(field.Name, (float)value);
        }
        else if (field.FieldType == typeof(bool))
        {
            value = EditorGUILayout.Toggle(field.Name, (bool)value);
        }
        else if (field.FieldType == typeof(Vector2))
        {
            value = EditorGUILayout.Vector2Field(field.Name, (Vector2)value);
        }
        else if (field.FieldType == typeof(RectTransform))
        {
            value = EditorGUILayout.ObjectField(field.Name, (RectTransform)value, typeof(RectTransform), true);
        }
        
        if (value != null && (origin == null || !origin.Equals(value)))
        {
            field.SetValue(_clip, value);
            EditorUtility.SetDirty(_componentEditor.Component);
        }
    }
}