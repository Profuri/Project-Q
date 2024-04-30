using System;
using System.Collections.Generic;
using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

[Flags]
public enum RGBColor
{
    None = 0,
    Red = 1,
    Green = 2,
    Blue = 4,
}

public class RGBObjectUnit : InteractableObject, IPassable
{
    private const int AllMatchColor = (int)(RGBColor.Red | RGBColor.Blue | RGBColor.Green);

    [Header("Color")]
    public RGBColor originColor;
    private RGBColor _addedColor;
    private RGBColor HasColor => originColor | _addedColor;
    protected bool MatchRGB => (int)HasColor == AllMatchColor;

    [Header("Layer")]
    [SerializeField] private LayerMask _targetLayer;
    
    private MeshRenderer _renderer;

    public bool PassableAfterAxis { get; set; }
    
    private static readonly int BaseColorHash = Shader.PropertyToID("_BaseColor");
    private static readonly int EmissionColorHash = Shader.PropertyToID("_EmissionColor");
    private static readonly int VisibleHash = Shader.PropertyToID("_VisibleProgress");

    public override void Awake()
    {
        base.Awake();
        if(_renderer == null)
        {
            _renderer = GetComponentInChildren<MeshRenderer>();
        }
        SettingColor();
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);

        SettingColor();
        SettingCollider();
    }
    
    public override void Convert(AxisType axis)
    {
        base.Convert(axis);
        SettingColor();
    }

    public override void ApplyUnitInfo(AxisType axis)
    {
        base.ApplyUnitInfo(axis);
        SettingCollider();
    }
    
    public void PassableCheck(AxisType axis)
    {
        if(AxisType.None == axis)
        {
            _addedColor = RGBColor.None;
        }
        else
        {
            FindRGBUnit(axis);
        }

        PassableAfterAxis = !MatchRGB;
    }
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
    }

    private void SettingColor()
    {
        var color = GetColorFromRGBColor(HasColor);
        var alpha = 1f - color.a;
        
        _renderer.materials[0].SetColor(BaseColorHash, color);
        _renderer.materials[0].SetColor(EmissionColorHash, color * 3f);

        foreach (var material in _renderer.materials)
        {
            material.SetFloat(VisibleHash, alpha);
        }
    }

    private void SettingCollider()
    {
        int layer;
        if(MatchRGB)
        {
            layer = 0;
            if(Rigidbody != null)
                Rigidbody.isKinematic = true;
        }
        else
        {
            layer = LayerMask.GetMask("Player");
            if(Rigidbody != null)
                Rigidbody.isKinematic = false;
        }
        Collider.excludeLayers = layer;
    }

    private Color GetColorFromRGBColor(RGBColor rgb)
    {
        var color = new Color(0, 0, 0, 0);
        var addedCount = 0;

        if (rgb.HasFlag(RGBColor.Red))
        {
            addedCount++;
            color.r = 1;
        }
        if (rgb.HasFlag(RGBColor.Blue))
        {
            addedCount++;
            color.b = 1;
        }
        if (rgb.HasFlag(RGBColor.Green))
        {
            addedCount++;
            color.g = 1;
        }

        if (addedCount > 0)
        {
            color.a = 0.7f + (addedCount - 1) * 0.15f;
        }

        return color;
    }

    private void FindRGBUnit(AxisType axis)
    {
        if (MatchRGB) return;
        Vector3 direction = Vector3ExtensionMethod.GetAxisDir(axis);
        Vector3 center = transform.position;
        Vector3 halfExtents = Collider.bounds.extents * 0.5f;
        Quaternion rotation = transform.rotation;

        List<RaycastHit> hitInfoList = new List<RaycastHit>();

        var frontInfos = Physics.BoxCastAll(center, halfExtents, direction, rotation, Mathf.Infinity, _targetLayer);
        var backInfos = Physics.BoxCastAll(center, halfExtents, -direction, rotation, Mathf.Infinity, _targetLayer);

        for (int i = 0; i < frontInfos.Length; i++)
            hitInfoList.Add(frontInfos[i]);
        for(int i = 0; i < backInfos.Length; i++)
            hitInfoList.Add(backInfos[i]);

        if (hitInfoList.Count > 0)
        {
            foreach (RaycastHit hitInfo in hitInfoList)
            {
                if (hitInfo.collider == Collider) continue;
                if(hitInfo.collider.TryGetComponent(out RGBObjectUnit rgbUnit))
                {
                    TransitionRGB(rgbUnit.originColor);
                }
            }
        }
    }

    private void TransitionRGB(RGBColor rgb)
    {
        _addedColor |= rgb;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = GetColorFromRGBColor(originColor);

        var col = GetComponent<Collider>();
        var bounds = col.bounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size * 1.1f);
    }

#endif
}
