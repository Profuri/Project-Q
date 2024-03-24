using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class SettingButton : InteractableObject
{
    [SerializeField] private Transform _titleCanvasTrm;
    private SettingWindow _settingWindow;
    
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        if (_settingWindow is null || !_settingWindow.poolOut)
        {
            _settingWindow = UIManager.Instance.GenerateUI("SettingWindow", _titleCanvasTrm) as SettingWindow;
            _settingWindow.transform.localPosition = new Vector3(5.05f, 1.2f, 7.48f);
            _settingWindow.transform.localRotation = Quaternion.identity;
        }
    }
}