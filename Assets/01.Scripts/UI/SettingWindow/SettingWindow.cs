using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public enum SettingType
{
    None = 0,
    Game,
    Audio,
    Control,
    Video,
    Count
}

public class SettingWindow : UIComponent
{
    [SerializeField] private WindowPanel[] _panels;
    private WindowPanel _currentPanel;

    protected override void Awake()
    {
        base.Awake();
        _panels = new WindowPanel[(int)SettingType.Count];
        foreach (SettingType type in Enum.GetValues(typeof(SettingType)))
        {
            if (type == SettingType.Count)
            {
                continue;
            }

            var panelName = $"Canvas/{type}SettingPanel";
            var panel = transform.Find(panelName).GetComponent<WindowPanel>();
            panel.Init(this);
            _panels[(int)type] = panel;
        }
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        _currentPanel?.ReleasePanel();
        _currentPanel = _panels[(int)SettingType.None];
        _currentPanel.LoadPanel();

        base.Appear(parentTrm, callback);

        SoundManager.Instance.PlaySFX("PanelAppear", false);
        CursorManager.RegisterUI(this);
    }

    public void Close()
    {
        _currentPanel?.ReleasePanel();
        SoundManager.Instance.PlaySFX("PanelDisappear", false);
        Disappear();
    }

    public override void Disappear(Action callback = null)
    {
        _currentPanel?.ReleasePanel();
        _currentPanel = _panels[(int)SettingType.None];
        _currentPanel.LoadPanel();
        
        CursorManager.UnRegisterUI(this);
        
        base.Disappear(callback);
    }
    
    public void ChangePanel(int type)
    {
        PlayTapSound();
        _currentPanel?.ReleasePanel();
        _currentPanel = _panels[type];
        _currentPanel.LoadPanel();
    }

    private void PlayTapSound()
    {
        SoundManager.Instance.PlaySFX("UITap");
    }
}