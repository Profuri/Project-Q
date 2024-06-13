using System;
using TMPro;
using UnityEngine;

public class ThanksToPanel : UIComponent
{
    private TextMeshProUGUI _playTimeText;

    protected override void Awake()
    {
        _playTimeText = transform.Find("PlayTimeText").GetComponent<TextMeshProUGUI>();
        base.Awake();
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, callback);
        
        var currentTimeTick = DateTime.Now.Ticks;
        var diffTicks = currentTimeTick - DataManager.sSaveData.GameStartTimeTick;
        var diffSpan = new TimeSpan(diffTicks);

        _playTimeText.text = $"you cleared in [ {diffSpan.Hours}:{diffSpan.Minutes}:{diffSpan.Seconds} ]";

        InputManager.Instance.UIInputReader.OnAnyKeyClickEvent += GoTitle;
    }

    public override void OnPush()
    {
        InputManager.Instance.UIInputReader.OnAnyKeyClickEvent -= GoTitle;
        InputManager.Instance.SetEnableInputAll(true);
        base.OnPush();
    }

    private void GoTitle()
    {
        SceneControlManager.Instance.LoadScene(SceneType.Title);
    }
}