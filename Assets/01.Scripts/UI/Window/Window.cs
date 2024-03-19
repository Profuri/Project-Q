using AxisConvertSystem;
using UnityEngine;

public class Window : ObjectUnit
{
    [SerializeField] private Canvas _canvas;
    
    public override void Init(AxisConverter converter)
    {
        base.Init(converter);
        _canvas.worldCamera = CameraManager.Instance.MainCam;
    }
}