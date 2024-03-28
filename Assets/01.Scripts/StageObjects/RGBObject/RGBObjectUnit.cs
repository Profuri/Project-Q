using System;
using System.Collections.Generic;
using System.Linq;
using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

[Flags]
public enum RGBColor
{
    NONE = 0,
    RED = 1,
    GREEN = 2,
    BLUE = 4,
}

public class RGBObjectUnit : InteractableObject
{
    private static readonly int s_allMatchColor = (int)(RGBColor.RED | RGBColor.BLUE |RGBColor.GREEN);

    [Header("Color")]
    public RGBColor originColor;
    private RGBColor _affectedColor;

    [Header("Layer")]
    [SerializeField] private LayerMask _targetLayer;
    
    private MeshRenderer _renderer;
    
    public RGBColor HasColor => (originColor | _affectedColor);
    public bool MatchRGBColor => (int)HasColor == s_allMatchColor;

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
        SettingColor(HasColor);
    }

    public override void Init(AxisConverter converter)
    {
        base.Init(converter);

        SettingColor(HasColor);
        SettingCollider();
    }

    private void SettingColor(RGBColor rgbColor)
    {
        var color = GetColorFromRGBColor(rgbColor);
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
        if(MatchRGBColor)
        {
            Collider.isTrigger = false;
        }
        else
        {
            Collider.isTrigger = true;
        }
    }

    private Color GetColorFromRGBColor(RGBColor color)
    {
        switch(color)
        {
            case RGBColor.RED:
                return new Color(1, 0, 0, 0.7f);
            case RGBColor.GREEN:
                return new Color(0, 1, 0, 0.7f);
            case RGBColor.BLUE:
                return new Color(0, 0, 1, 0.7f);
            case RGBColor.RED | RGBColor.GREEN:
                return new Color(1, 1, 0, 0.85f);
            case RGBColor.GREEN | RGBColor.BLUE:
                return new Color(0, 1, 1, 0.85f);
            case RGBColor.RED | RGBColor.BLUE:
                return new Color(1, 0, 1, 0.85f);
            case RGBColor.RED | RGBColor.GREEN | RGBColor.BLUE:
                return new Color(1, 1, 1, 1f);
            default:
                return Color.clear;
        }
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        try
        {
            RGBColor rgbColor = (RGBColor)param[0];

            if (interactValue)
            {
                TransitionRGB(rgbColor);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError($"Can't convert to rgb Color! Message: {ex.Message}");
        }
    }

    private void FindRGBUnit(AxisType axis)
    {
        if (MatchRGBColor) return;
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

    private void TransitionRGB(RGBColor color)
    {
        _affectedColor = _affectedColor | color;
    }

    //이건 위치 값 변하기 전
    public override void Convert(AxisType axis)
    {
        base.Convert(axis);
        if(AxisType.None == axis)
        {
            _affectedColor = RGBColor.NONE;
        }
        else
        {
            FindRGBUnit(axis);
        }

        SettingCollider();
        SettingColor(HasColor);
    }
    
#if UNITY_EDITOR

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = GetColorFromRGBColor(originColor);

        var col = GetComponent<Collider>();
        var bounds = col.bounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size * 1.1f);
    }

#endif
}
