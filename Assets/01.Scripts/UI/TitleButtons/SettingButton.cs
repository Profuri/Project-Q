using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class SettingButton : InteractableObject
{
    [SerializeField] private Transform _titleCanvasTrm;
    private SettingWindow _settingWindow;
    
    [SerializeField] private UIButton3D _button3D;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        SettingButtonCall();
    }
    
    public override void OnDetectedEnter(ObjectUnit communicator = null)
    {
        base.OnDetectedEnter(communicator);
        if (_button3D != null)
        {
            _button3D.OnHoverHandle();
        }
    }

    public override void OnDetectedLeave(ObjectUnit communicator = null)
    {
        base.OnDetectedLeave(communicator);
        if (_button3D != null)
        {
            _button3D.OnHoverCancelHandle();
        }
    }

    public void SettingButtonCall()
    {
        if (_settingWindow is null || !_settingWindow.poolOut)
        {
            _settingWindow = UIManager.Instance.GenerateUI("SettingWindow", _titleCanvasTrm) as SettingWindow;
            _settingWindow.transform.localPosition = new Vector3(5.05f, 1.2f, 7.48f);
            _settingWindow.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
        }
    }
}