using System;
using System.Collections.Generic;
using System.Drawing;
using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;


[Flags,Serializable]
public enum RGBColor
{
    RED,BLUE,GREEN
}

public class RGBObjectUnit : InteractableObject
{

    [Header("Color")]
    public RGBColor originColor;
    private RGBColor _hasColor;

    [Header("Layer")]
    [SerializeField] private LayerMask _targetLayer;
    //이거 Ray 뒤에서 쏴야됨
    [SerializeField] private float _rayLength = 10f;

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        try
        {
            RGBColor rgbColor = (RGBColor)param[0];

            if (interactValue)
            {
                TransitionRGB(rgbColor);
            }
            else
            {
                UnTransitionRGB(rgbColor);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError($"Can't change to rgb Color! Message: {ex.Message}");
        }
    }

    private void FindRGBUnit(AxisType axis)
    {
        Vector3 direction = Vector3ExtensionMethod.GetAxisDir(axis);
        Vector3 center = transform.position;
        Vector3 halfExtents = Collider.bounds.extents * 0.5f;
        Quaternion rotation = transform.rotation;

        RaycastHit[] hitInfos = Physics.BoxCastAll(center, halfExtents, direction, rotation, _rayLength,_targetLayer);

        if (hitInfos.Length > 0)
        {
            List<RGBObjectUnit> rgbUnitList = new List<RGBObjectUnit>();
            foreach (RaycastHit hitInfo in hitInfos)
            {
                if(hitInfo.collider.TryGetComponent(out RGBObjectUnit rgbUnit))
                {
                    rgbUnitList.Add(rgbUnit);
                    TransitionRGB(rgbUnit.originColor);
                }
            }
            rgbUnitList.ForEach(unit => unit.OnInteraction(this,true,_hasColor));
        }

        Debug.Log($"Current Color: {_hasColor}");
    }

    private void TransitionRGB(RGBColor color)
    {
        _hasColor |= color;
    }

    private void UnTransitionRGB(RGBColor color)
    {
        _hasColor |= ~color;
    }


    //이건 위치 값 변하기 전
    public override void Convert(AxisType axis)
    {
        base.Convert(axis);
        FindRGBUnit(axis);
    }


    //이건 값 변경한 후
    public override void UnitSetting(AxisType axis)
    {
        base.UnitSetting(axis);
    }
}
