using System;
using UnityEngine;

public class UISliderHolder3D : UIButton3D, IClickUpHandler
{
    private LayerMask _sliderLineMask;
    public UISlider3D Slider { get; set; }
    
    private bool _isHold;
    
    protected override void Awake()
    {
        base.Awake();
        _sliderLineMask = LayerMask.GetMask("SliderLine");
    }

    private void Update()
    { if (_isHold)
        {
            Slider.SetProgress(GetHolderValue());
        }
    }

    private float GetHolderValue()
    {
        var screenMousePoint = InputManager.Instance.InputReader.mouseScreenPoint;
        var ray = CameraManager.Instance.MainCam.ScreenPointToRay(screenMousePoint);

        var isHit = Physics.Raycast(ray, out var hit, Mathf.Infinity, _sliderLineMask);

        if (isHit)
        {
            var adjustHitPoint = new Vector3(hit.point.x, Slider.LineMinTrm.position.y, hit.point.z);
            var pointVector = adjustHitPoint - Slider.LineMinTrm.position;
            var scalar = Vector3.Dot(pointVector, Slider.LineVector) / Slider.LineVector.magnitude;

            return scalar / Slider.LineVector.magnitude;
        }
        else
        {
            return Slider.Percent;
        }
    }

    public override void OnClickHandle()
    {
        _isHold = true;
        base.OnClickHandle();
    }

    public void OnClickUpHandle()
    {
        _isHold = false;
    }
}