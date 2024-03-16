using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIComponent))]
public class UIComponentEditor : Editor
{
    public UIComponent Component { get; private set; }
    
    private List<Type> _animationTypes;                                             // READONLY
    private Dictionary<UIAnimation, UIAnimationHandler> _uiAnimationEditors;        // READONLY

    private void OnEnable()
    {
        Component = (UIComponent)target;
        
        _animationTypes = new List<Type>();
        _uiAnimationEditors = new Dictionary<UIAnimation, UIAnimationHandler>();

        if (Component.tweenData != null)
        {
            InitClipDictionary(Component.tweenData.appearAnimator);
            InitClipDictionary(Component.tweenData.disappearAnimator);
        }
        
        LoadUIAnimationType();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Component.tweenData != null)
        {
            DrawAnimatorInspector("Appear Animations", ref Component.tweenData.appearAnimator);
            DrawAnimatorInspector("Disappear Animations", ref Component.tweenData.disappearAnimator);
        }
        else
        {
            DrawCreateDataButton();
        }
    }

    private void DrawCreateDataButton()
    {
        if (GUILayout.Button("Create New Data"))
        {
            CreateNewTweenData();
        }
    }

    private void CreateNewTweenData()
    {
        var assetPath = $"Assets/07.ScriptableObjects/UITweenData/{Component.gameObject.name}TweenData.asset";
        var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        var asset = ScriptableObject.CreateInstance<UIComponentTweenData>();
        asset.appearAnimator = new UIAnimator();
        asset.disappearAnimator = new UIAnimator();

        AssetDatabase.CreateAsset(asset, uniqueAssetPath);
        EditorUtility.SetDirty(asset);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        Component.tweenData = asset;
        EditorUtility.SetDirty(Component);
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
            while (idx < animator.clips.Count)
            {
                _uiAnimationEditors[animator.clips[idx++]].DrawClipGUI();
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
                AddClip(animator, GenerateClipAsset(type), animator.clips.Count);
            });
        }
    }

    private UIAnimation GenerateClipAsset(Type type)
    {
        var clip = ScriptableObject.CreateInstance(type);
        clip.name = type.ToString();
        AssetDatabase.AddObjectToAsset(clip, Component.tweenData);
        EditorUtility.SetDirty(Component.tweenData);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        return (UIAnimation)clip;
    }

    private void AddClip(UIAnimator animator, UIAnimation clip, int index)
    {
        animator.clips.Add(clip);
        AddClipToEditor(animator, clip, index);
    }

    private void AddClipToEditor(UIAnimator animator, UIAnimation clip, int index)
    {
        var handler = new UIAnimationHandler(animator, clip, index, this);
        _uiAnimationEditors.Add(clip, handler); 
    }

    public void RemoveClip(UIAnimator animator, UIAnimation clip)
    {
        animator.clips.Remove(clip);
        _uiAnimationEditors.Remove(clip);
        
        Undo.DestroyObjectImmediate(clip);
        EditorUtility.SetDirty(Component.tweenData);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private void InitClipDictionary(UIAnimator animator)
    {
        for (var i = 0; i < animator.clips.Count; i++)
        {
            AddClipToEditor(animator, animator.clips[i], i);
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