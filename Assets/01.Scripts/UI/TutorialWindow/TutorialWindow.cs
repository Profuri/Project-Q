using System;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialWindow : UIComponent
{
    private VideoPlayer _videoPlayer;
    private TextMeshPro _mainText;
    private TextMeshPro _descText;

    protected override void Awake()
    {
        base.Awake();
        var canvasTrm = transform.Find("Canvas");
        _videoPlayer = canvasTrm.Find("VideoPanel/Player").GetComponent<VideoPlayer>();
        _mainText = canvasTrm.Find("MainTutText").GetComponent<TextMeshPro>();
        _descText = canvasTrm.Find("DescTutText").GetComponent<TextMeshPro>();
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, callback);
        SoundManager.Instance.PlaySFX("PanelAppear", false);
    }
    public override void Disappear(Action callback = null)
    {
        _videoPlayer.Stop();
        base.Disappear(callback);
        SoundManager.Instance.PlaySFX("PanelAppear", false);
    }

    public void SettingTutorial(TutorialInfo info)
    {
        _videoPlayer.clip = info.clip;
        _mainText.text = info.mainText;
        _descText.text = info.descText;
    }

    public void PlayTutorial()
    {
        _videoPlayer.Play();
    }
}