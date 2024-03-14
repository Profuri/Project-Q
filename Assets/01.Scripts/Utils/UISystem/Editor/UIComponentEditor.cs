using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIComponent))]
public class UIComponentEditor : Editor
{
    public UIComponent Component { get; private set; }
    
    private bool _test;
    
    private List<Type> _animationTypes;
    private Dictionary<UIAnimation, UIAnimationHandler> _uiAnimationEditors;

    private Dictionary<UIAnimator, List<UIAnimation>> _removedClips;

    private void OnEnable()
    {
        Component = (UIComponent)target;
        _animationTypes = new List<Type>();
        _uiAnimationEditors = new Dictionary<UIAnimation, UIAnimationHandler>();
        _removedClips = new Dictionary<UIAnimator, List<UIAnimation>>();
        
        // 딕셔너리에 초기값 담아주는 로직 제작해야함
        
        // for test
        Component.appearAnimator.Clips.Clear();
        Component.disappearAnimator.Clips.Clear();
        
        LoadUIAnimationType();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        DrawAnimatorInspector("Appear Animations", ref Component.appearAnimator);
        DrawAnimatorInspector("Disappear Animations", ref Component.disappearAnimator);
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
            ApplyRemoveResult();
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
                AddClip(animator, clip);
            });
        }
    }

    private void AddClip(UIAnimator animator, UIAnimation clip)
    {
        var handler = new UIAnimationHandler(animator, clip, this);
        animator.Clips.Add(clip);
        _uiAnimationEditors.Add(clip, handler);
        EditorUtility.SetDirty(Component);
    }

    public void RemoveClip(UIAnimator animator, UIAnimation clip)
    {
        if (!_removedClips.ContainsKey(animator))
        {
            _removedClips.Add(animator, new List<UIAnimation>());
        }
        _removedClips[animator].Add(clip);
    }

    private void ApplyRemoveResult()
    {
        foreach (var animator in _removedClips.Keys)
        {
            foreach (var clip in _removedClips[animator])
            {
                animator.Clips.Remove(clip);
                _uiAnimationEditors.Remove(clip);
            }
            _removedClips[animator].Clear();
        }
        EditorUtility.SetDirty(Component);
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