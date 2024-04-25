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
            _panels[(int)type] = panel;
        }
    }

    private void Start()
    {
        foreach (var panel in _panels)
        {
            panel.Init(this);
        }
        ChangePanel((int)SettingType.None);
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, callback);

        SoundManager.Instance.PlaySFX("PanelPopup", false);
        ChangePanel((int)SettingType.None);
        CursorManager.RegisterUI(this);
    }

    public void Close()
    {
        _currentPanel?.ReleasePanel();
        SoundManager.Instance.PlaySFX("PanelClose", false);
        Disappear();
    }

    public override void Disappear(Action callback = null)
    {
        base.Disappear(callback);
        CursorManager.UnRegisterUI(this);
    }
    
    [VisibleEnum(typeof(SettingType))]
    public void ChangePanel(int type,bool isSound = false)
    {
        if(isSound)
            SoundManager.Instance.PlaySFX("UITap", false);

        _currentPanel?.ReleasePanel();
        _currentPanel = _panels[type];
        _currentPanel.LoadPanel();
    }
}