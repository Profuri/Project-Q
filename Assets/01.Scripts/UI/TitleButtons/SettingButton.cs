using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class SettingButton : InteractableObject
{
    [SerializeField] private SettingWindow _window;
    [SerializeField] private Transform _canvas;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        _window.Appear(_canvas);
    }
}