using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIComponent))]
public class UIComponentEditor : Editor
{
    private UIComponent _component;
    
    private bool _test;
    private List<Type> _animationTypes;
    private Dictionary<UIAnimation, UIAnimationHandler> _uiAnimationEditors;

    private void OnEnable()
    {
        _component = (UIComponent)target;
        _animationTypes = new List<Type>();
        _uiAnimationEditors = new Dictionary<UIAnimation, UIAnimationHandler>();
        
        // 딕셔너리에 초기값 담아주는 로직 제작해야함
        
        // for test
        _component.appearAnimator.Clips.Clear();
        _component.disappearAnimator.Clips.Clear();
        
        LoadUIAnimationType();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        DrawAnimatorInspector("Appear Animations", ref _component.appearAnimator);
        DrawAnimatorInspector("Disappear Animations", ref _component.disappearAnimator);
    }

    private void DrawAnimatorInspector(string title, ref UIAnimator animator)
    {
        EditorGUILayout.Space(10);
        GUILayout.Label(title, EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.Space(2);

            EditorGUILayout.Space(1f);
            foreach (var clip in animator.Clips)
            {
                _uiAnimationEditors[clip].DrawClipGUI();
            }
            EditorGUILayout.Space(1f);

            if (GUILayout.Button("Add Animation Clip"))
            {
                GetAnimationTypeMenu(animator, out var menu);
                menu.ShowAsContext();
            }
            
            EditorGUILayout.Space(2);
        }
        EditorGUILayout.EndVertical();
    }

    private void GetAnimationTypeMenu(UIAnimator animator, out GenericMenu menu)
    {
        menu = new GenericMenu();
        foreach (var type in _animationTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () =>
            {
                var clip = (UIAnimation)Activator.CreateInstance(type);
                var handler = new UIAnimationHandler(clip, _component);
                animator.Clips.Add(clip);
                _uiAnimationEditors.Add(clip, handler);
            });
        }
    }

    private void LoadUIAnimationType()
    {
        var assembly = Assembly.GetAssembly(typeof(UIAnimation));
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (type.IsSubclassOf(typeof(UIAnimation)))
            {
                _animationTypes.Add(type);
            }
        }
    }
}