using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
using TMPro;
using System.Runtime.CompilerServices;


public class TutorialCanvas : PoolableMono
{
    private VideoPlayer _videoPlayer;
    private RawImage _videoImage;

    private TextMeshProUGUI _infoText;
    private TutorialSO _tutorialSO;

    private Action OnVideoEnd;
    private Action<int> OnLoadVideo;

    private IndexButtonController _indexBtnController;

    public override void OnPop()
    {
        Transform videoPlayerTrm = transform.Find("TutorialViewer/VideoPlayer");
        Transform tutorialInfoTrm = transform.Find("TutorialViewer/TutorialInfo");

        _videoPlayer = videoPlayerTrm.GetComponent<VideoPlayer>();
        _videoImage = videoPlayerTrm.GetComponent<RawImage>();

        Button prevBtn = tutorialInfoTrm.Find("Buttons/PrevBtn").GetComponent<Button>();
        Button nextBtn = tutorialInfoTrm.Find("Buttons/NextBtn").GetComponent<Button>();

        _infoText = tutorialInfoTrm.Find("TutorialText").GetComponent<TextMeshProUGUI>();
        _indexBtnController = new IndexButtonController(prevBtn,nextBtn,0);

        _videoImage.enabled = false;

        _videoPlayer.loopPointReached += VideoEnd;
    }

    public override void OnPush()
    {
        TutorialManager.Instance.IsTutorialViewing = false;
        _videoImage.enabled = false;
        _videoPlayer.loopPointReached -= VideoEnd;
        _indexBtnController.OnIndexChanged -= StartVideo;
        OnVideoEnd = null;
    }

    public void ShowTutorial(TutorialSO tutorialSO)
    {
        TutorialManager.Instance.IsTutorialViewing = true;
        _indexBtnController.IndexCnt = tutorialSO.tutorialList.Count;
        _indexBtnController.OnIndexChanged += StartVideo;
        _tutorialSO = tutorialSO;
        ShowSequence(() =>
        {
            StartVideo(0);
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

    private void StartVideo(int index)
    {
        _videoPlayer.Stop();

        VideoClip clip = _tutorialSO.tutorialList[index].videoClip;
        string infoText = _tutorialSO.tutorialList[index].information;

        _infoText.SetText(infoText);

        _videoImage.enabled = true;
        _videoPlayer.playOnAwake = true;
        _videoPlayer.clip = clip;

        _videoPlayer.Play();
    }




    private void VideoEnd(VideoPlayer source) => OnVideoEnd?.Invoke();
}


public class IndexButtonController
{
    public IndexButtonController(Button prevBtn, Button nextBtn, int indexCnt)
    {
        _index = 0;
        _prevBtn = prevBtn;
        _nextBtn = nextBtn;

        IndexCnt = indexCnt;

        _prevBtn.onClick.AddListener(() =>
        {
            if (_index > 0)
            {
                _index -= 1;
                OnIndexChanged?.Invoke(_index);
            }
        });
        _nextBtn.onClick.AddListener(() =>
        {
            if (_index < IndexCnt - 1)
            {
                _index += 1;
                OnIndexChanged?.Invoke(_index);
            }
        });
    }

    private Button _prevBtn;
    private Button _nextBtn;

    private int _index;

    private int _indexCnt;
    public int IndexCnt
    {
        get => _indexCnt;
        set
        {
            if(value > 1)
            {
                _prevBtn.gameObject.SetActive(true);
                _nextBtn.gameObject.SetActive(true);
            }
            else
            {
                _prevBtn.gameObject.SetActive(false);
                _nextBtn.gameObject.SetActive(false);
            }
            _indexCnt = value;
        }
    }


    public event Action<int> OnIndexChanged;
}
