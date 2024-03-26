using System;
using TinyGiantStudio.Text;
using UnityEngine.Video;

public class TutorialWindow : UIComponent
{
    private VideoPlayer _videoPlayer;
    private Modular3DText _mainText;
    private Modular3DText _descText;

    protected override void Awake()
    {
        base.Awake();
        var canvasTrm = transform.Find("Canvas");
        _videoPlayer = canvasTrm.Find("VideoPanel/Player").GetComponent<VideoPlayer>();
        _mainText = canvasTrm.Find("MainTutText").GetComponent<Modular3DText>();
        _descText = canvasTrm.Find("DescTutText").GetComponent<Modular3DText>();
    }

    public override void Disappear(Action callback = null)
    {
        _videoPlayer.Stop();
        base.Disappear(callback);
    }

    public void SettingTutorial(TutorialInfo info)
    {
        _videoPlayer.clip = info.clip;
        _mainText.Text = info.mainText;
        _descText.Text = info.descText;
    }

    public void PlayTutorial()
    {
        _videoPlayer.Play();
    }
}