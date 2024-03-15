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
    
    private List<Type> _animationTypes;                                             // READONLY
    private Dictionary<UIAnimation, UIAnimationHandler> _uiAnimationEditors;        // READONLY

    private void OnEnable()
    {
        Component = (UIComponent)target;
        _animationTypes = new List<Type>();
        _uiAnimationEditors = new Dictionary<UIAnimation, UIAnimationHandler>();
        
        InitClipDictionary(Component.appearAnimator);
        InitClipDictionary(Component.disappearAnimator);
        
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
            var idx = 0;
            while (idx < animator.Clips.Count)
            {
                _uiAnimationEditors[animator.Clips[idx++]].DrawClipGUI();
            }
            EditorGUILayout.Space(1f);

            EditorGUILayout.BeginHorizontal();
            {
                if (Application.isPlaying)
                {
                    if (GUILayout.Button("Preview"))
                    {
                        Debug.Log("preview");
                        animator.Play();
                    }
                }
                if (GUILayout.Button("Add Animation Clip"))
                {
                    GetAnimationTypeMenu(animator, out var menu);
                    menu.ShowAsContext();
                }
            }
            EditorGUILayout.EndHorizontal();
            
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
        animator.Clips.Add(clip);
        AddClipToEditor(animator, clip);
        EditorUtility.SetDirty(Component);
    }

    private void AddClipToEditor(UIAnimator animator, UIAnimation clip)
    {
        var handler = new UIAnimationHandler(animator, clip, this);
        _uiAnimationEditors.Add(clip, handler); 
    }

    public void RemoveClip(UIAnimator animator, UIAnimation clip)
    {
        animator.Clips.Remove(clip);
        _uiAnimationEditors.Remove(clip);
        EditorUtility.SetDirty(Component);
    }

    private void InitClipDictionary(UIAnimator animator)
    {
        foreach (var clip in animator.Clips)
        {
            AddClipToEditor(animator, clip);
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