using UnityEngine;

public class UISliderHolder3D : MonoBehaviour, IClickHandler, IClickUpHandler
{
    private LayerMask _sliderLineMask;
    public UISlider3D Slider { get; set; }
    
    private bool _isHold;
    private float _offset;

    private void Awake()
    {
        _sliderLineMask = LayerMask.GetMask("SliderLine");
    }

    private void Update()
    {
        if (_isHold)
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
            var pointVector = hit.point - Slider.LineMinTrm.position;
            var scalar = Vector3.Dot(pointVector, Slider.LineVector) / Slider.LineVector.magnitude;

            return scalar / Slider.LineVector.magnitude;
        }
        else
        {
            return Slider.Percent;
        }
    }

    public void OnClickHandle()
    {
        _isHold = true;
    }

    public void OnClickUpHandle()
    {
        _isHold = false;
    }
}