using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIDissolveClip : UIAnimation
{
    public bool dissolve;
    private List<Material> _materials;

    private float _curProgress;
    
    private readonly int _dissolveProgressHash = Shader.PropertyToID("_DissolveProgress");

    private void OnEnable()
    {
        _materials = new List<Material>();
    }

    public override void Init()
    {
        _materials.Clear();
        var renderers = targetTrm.GetComponents<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                _materials.Add(material);
            }
        }
        
        _curProgress = dissolve ? 0f : 1f;
        foreach (var material in _materials)
        {
            material.SetFloat(_dissolveProgressHash, _curProgress);
        }
    }

    public override Tween GetTween()
    {
        return DOTween.To(() => _curProgress, progress =>
        {
            foreach (var material in _materials)
            {
                material.SetFloat(_dissolveProgressHash, progress);
            }
        }, dissolve ? 1f : 0f, duration);
    }
}