using System;
using UnityEngine;

public class UISlider3D : UIComponent
{
    private UISliderHolder3D _holder;

    public Transform LineMinTrm { get; private set; }
    public Transform LineMaxTrm { get; private set; }
    public Vector3 LineVector => LineMaxTrm.position - LineMinTrm.position;
    
    private Transform _progressPivot;

    public float Percent { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        LineMinTrm = transform.Find("Min");
        LineMaxTrm = transform.Find("Max");

        _holder = transform.GetComponentInChildren<UISliderHolder3D>();
        _holder.Slider = this;
        
        _progressPivot = transform.Find("Pivot");
        
        SetProgress(0f);
    }

    public void SetProgress(float percent)
    {
        Percent = Mathf.Clamp(percent, 0f, 1f);

        var holderLocalPos = _holder.transform.localPosition;
        holderLocalPos.z = Mathf.Lerp(LineMinTrm.localPosition.z, LineMaxTrm.localPosition.z, Percent);
        _holder.transform.localPosition = holderLocalPos;

        var pivotScale = _progressPivot.localScale;
        pivotScale.z = Percent;
        _progressPivot.localScale = pivotScale;
    }
}
