using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class UIAnimationHandler
{
    private readonly UIAnimator _animator;
    private readonly UIComponentEditor _componentEditor;
    private readonly UIAnimation _clip;
    private readonly List<FieldInfo> _fields;
    
    private int _index;
    private bool _foldValue;
    
    public UIAnimationHandler(UIAnimator animator, UIAnimation clip, int index, UIComponentEditor componentEditor)
    {
        _animator = animator;
        _clip = clip;
        _componentEditor = componentEditor;
        _index = index;
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
                GUILayout.Space(200);
                if (GUILayout.Button("↑"))
                {
                    UpBtnHandler();
                }
                if (GUILayout.Button("↓"))
                {
                    DownButtonHandler();
                }
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

    private void UpBtnHandler()
    {
        if (_index - 1 < 0)
        {
            return;
        }
        _animator.clips.Swap(_index, --_index);
    }

    private void DownButtonHandler()
    {
        if (_index + 1 >= _animator.clips.Count)
        {
            return;
        }
        _animator.clips.Swap(_index, ++_index);
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
        else if (field.FieldType == typeof(string))
        {
            value = EditorGUILayout.TextField(field.Name, (string)value);
        }
        else if (field.FieldType == typeof(bool))
        {
            value = EditorGUILayout.Toggle(field.Name, (bool)value);
        }
        else if (field.FieldType == typeof(Vector2))
        {
            value = EditorGUILayout.Vector2Field(field.Name, (Vector2)value);
        }
        else if (field.FieldType == typeof(Vector3))
        {
            value = EditorGUILayout.Vector3Field(field.Name, (Vector3)value);
        }
        else if (field.FieldType == typeof(Transform))
        {
            value = EditorGUILayout.ObjectField(field.Name, (Transform)value, typeof(Transform), true);
        }
        else if (field.FieldType == typeof(RectTransform))
        {
            value = EditorGUILayout.ObjectField(field.Name, (RectTransform)value, typeof(RectTransform), true);
        }
        else if (field.FieldType == typeof(Ease))
        {
            value = EditorGUILayout.EnumPopup(field.Name, (Ease)value);
        }
        
        if (value != null && (origin == null || !origin.Equals(value)))
        {
            field.SetValue(_clip, value);
            EditorUtility.SetDirty(_componentEditor.Component.tweenData);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}