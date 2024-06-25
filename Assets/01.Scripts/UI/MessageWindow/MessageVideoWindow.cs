using System;
using UnityEngine.Video;

public class MessageVideoWindow : UIComponent
{
    private VideoPlayer _videoPlayer;
    
    protected override void Awake()
    {
        base.Awake();
        var canvasTrm = transform.Find("Canvas");
        _videoPlayer = canvasTrm.Find("VideoPanel/Player").GetComponent<VideoPlayer>();
    }

    public override void Disappear(Action<UIComponent> callback = null)
    {
        _videoPlayer.Stop();
        base.Disappear(callback);
    }

    public void SettingVideo(VideoClip clip)
    {
        _videoPlayer.clip = clip;
    }

    public void Play()
    {
        _videoPlayer.Play();
    }
}