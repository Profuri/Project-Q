using UnityEngine;

public class SettingWindow : UIComponent
{
    [SerializeField] private MainSettingPanel _mainSettingPanel;
    [SerializeField] private AudioSettingPanel _audioSettingPanel;
    [SerializeField] private ControlSettingPanel _controlSettingPanel;
    [SerializeField] private VideoSettingPanel _videoSettingPanel;
    
    private MonoBehaviour _currentPanel;

    protected override void Awake()
    {
        base.Awake();
        ChangePanel(_mainSettingPanel);
    }

    public override void Appear(Transform parentTrm)
    {
        base.Appear(parentTrm);
        ChangePanel(_mainSettingPanel);
    }

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

    private void ChangePanel(MonoBehaviour panel)
    {
        if (_currentPanel is not null)
        {
            _currentPanel.gameObject.SetActive(false);
        }
        
        panel.gameObject.SetActive(true);
        _currentPanel = panel;
    }
}