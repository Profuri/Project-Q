using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
using TMPro;

public class TutorialCanvas : PoolableMono
{
    private VideoPlayer _videoPlayer;
    private RawImage _videoImage;
    private TextMeshProUGUI _infoText;

    private Action OnVideoEnd;

    public override void OnPop()
    {
        Transform videoPlayerTrm = transform.Find("TutorialViewer/VideoPlayer");

        _videoPlayer = videoPlayerTrm.GetComponent<VideoPlayer>();
        _videoImage = videoPlayerTrm.GetComponent<RawImage>();
        _infoText = GetComponentInChildren<TextMeshProUGUI>();

        _videoImage.enabled = false;

        _videoPlayer.loopPointReached += VideoEnd;
    }

    public override void OnPush()
    {
        _videoImage.enabled = false;
        _videoPlayer.loopPointReached -= VideoEnd;
        OnVideoEnd = null;
    }

    public void ShowTutorial(TutorialSO tutorialSO)
    {
        ShowSequence(() =>
        {
            StartVideo(tutorialSO);
        });
    }

    private void ShowSequence(Action Callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_videoImage.transform.DOScale(Vector3.zero, 0f));
        seq.Append(_videoImage.transform.DOScale(Vector3.one, 0.3f));
        seq.AppendCallback(() =>
        {
            Callback?.Invoke();
        });
    }

    private void StartVideo(TutorialSO tutorialSO,int index = 0)
    {
        VideoClip clip = tutorialSO.tutorialList[index].videoClip;
        string infoText = tutorialSO.tutorialList[index].information;

        _infoText.SetText(infoText);

        _videoImage.enabled = true;
        _videoPlayer.playOnAwake = true;
        _videoPlayer.clip = clip;

        //.SetEase(Ease.InExpo)
        if(index < tutorialSO.tutorialList.Count - 1)
        {
            OnVideoEnd = () => StartVideo(tutorialSO, ++index);
        }
        else
        {
            OnVideoEnd = () => SceneControlManager.Instance.DeleteObject(this);
        }
    }
    private void VideoEnd(VideoPlayer source) => OnVideoEnd?.Invoke();
}
