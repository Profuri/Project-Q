using System;
using UnityEngine;

public class SettingWindow : UIComponent
{
    [SerializeField] private MainSettingPanel _mainSettingPanel;
    [SerializeField] private AudioSettingPanel _audioSettingPanel;
    [SerializeField] private ControlSettingPanel _controlSettingPanel;
    [SerializeField] private VideoSettingPanel _videoSettingPanel;
    
    private WindowPanel _currentPanel;

    private void Start()
    {
        _mainSettingPanel.Init(this);
        _audioSettingPanel.Init(this);
        _controlSettingPanel.Init(this);
        _videoSettingPanel.Init(this);
        ChangePanel(_mainSettingPanel);
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, callback);
        ChangePanel(_mainSettingPanel);
        CursorManager.RegisterUI(this);
    }

    public void Close()
    {
        _currentPanel?.ReleasePanel();
        Disappear();
    }

    public override void Disappear(Action callback = null)
    {
        base.Disappear(callback);
        CursorManager.UnRegisterUI(this);
    }

    // Button Callbacks
    public void GoMain()
    {
        ChangePanel(_mainSettingPanel);
    }

    public void GoAudioSetting()
    {
        ChangePanel(_audioSettingPanel);
    }

    public void GoVideoSetting()
    {
        ChangePanel(_videoSettingPanel);
    }

    public void GoControlSetting()
    {
        ChangePanel(_controlSettingPanel);
    }

    private void ChangePanel(WindowPanel panel)
    {
        _currentPanel?.ReleasePanel();
        _currentPanel = panel;
        _currentPanel.LoadPanel();
    }
}