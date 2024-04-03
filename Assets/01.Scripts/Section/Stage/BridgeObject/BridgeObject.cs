using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BridgeObject : PoolableMono
{
    private Section _section;
    private List<Material> _materials;

    private BoxCollider _collider;
        
    private readonly int _visibleProgressHash = Shader.PropertyToID("_VisibleProgress");

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        
        _materials = new List<Material>();
        var renderers = transform.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if(!renderer.enabled)
            {
                continue;
            }
            
            foreach (var material in renderer.materials) 
            {
                _materials.Add(material);    
            }
        }
    }

    public void SetWidth(float width)
    {
        var scale = transform.localScale;
        scale.z = width;
        transform.localScale = scale;
    }

    public void SetColliderSize(float intervalDistance)
    {
        var colSize = _collider.size;
        colSize.z = 1f + intervalDistance;
        _collider.size = colSize;
    }
    
    public void Generate(Vector3 position, Quaternion rotation, Section section)
    {
        transform.position = position - Vector3.up * 5;
        transform.rotation = rotation;
        _section = section;
        
        Dissolve(true, 2.5f);
        transform.DOMove(position, 3f);
    }

    public void Remove()
    {
        Dissolve(false, 3f);
        transform.DOMove(transform.position - Vector3.up * _section.SectionData.sectionYDepth, 3f)
            .OnComplete(() =>
            {
                SceneControlManager.Instance.DeleteObject(this);
            });
    }

    private void Dissolve(bool on, float time)
    {
        var value = on ? 0f : 1f;
        
        foreach (var material in _materials)
        {
            material.SetFloat(_visibleProgressHash, Mathf.Abs(1f - value));
        }

        foreach (var material in _materials)
        {
            DOTween.To(() => material.GetFloat(_visibleProgressHash),
                progress => material.SetFloat(_visibleProgressHash, progress), value, time);
        }
    }

    public override void OnPop()
    {
    }

    public override void OnPush()
    {
    }
}
