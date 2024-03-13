using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
using TMPro;

[System.Serializable]
public struct TutorialImage
{
    public MaskableGraphic image;
    public float targetAlphaValue;
}


public class TutorialCanvas : PoolableMono
{
    public bool IsShowing { get; private set;}

    private VideoPlayer _videoPlayer;
    private RawImage _videoImage;
    private Image _tutorialInfo;
    private Image _tutorialViewer;

    private TextMeshProUGUI _infoText;
    private TutorialSO _tutorialSO;

    private Action OnVideoEnd;

    private IndexButtonController _indexBtnController;

    [Header("Please enter a value between 0 and 1.")]
    [SerializeField] private List<TutorialImage> _tutorialImageList = new List<TutorialImage>();

    public override void OnPop()
    {
        Transform tutorialViewerTrm = transform.Find("TutorialViewer");
        _tutorialViewer = tutorialViewerTrm.GetComponent<Image>();

        Transform videoPlayerTrm = tutorialViewerTrm.Find("VideoPlayer");
        Transform tutorialInfoTrm = tutorialViewerTrm.Find("TutorialInfo");

        _tutorialInfo = tutorialInfoTrm.GetComponent<Image>();

        _videoPlayer = videoPlayerTrm.GetComponent<VideoPlayer>();
        _videoImage = videoPlayerTrm.GetComponent<RawImage>();

        Button prevBtn = tutorialInfoTrm.Find("Buttons/PrevBtn").GetComponent<Button>();
        Button nextBtn = tutorialInfoTrm.Find("Buttons/NextBtn").GetComponent<Button>();

        _infoText = tutorialInfoTrm.Find("TutorialText").GetComponent<TextMeshProUGUI>();
        _indexBtnController = new IndexButtonController(prevBtn,nextBtn,0);

        _videoImage.enabled = false;

        _videoPlayer.loopPointReached += VideoEnd;

        IsShowing = false;
    }

    public override void OnPush()
    {
        TutorialManager.Instance.IsTutorialViewing = false;
        _videoImage.enabled = false;
        _videoPlayer.loopPointReached -= VideoEnd;
        _indexBtnController.OnIndexChanged -= StartVideo;
        OnVideoEnd = null;
        IsShowing = false;
    }

    public void ShowTutorial(TutorialSO tutorialSO)
    {
        if (IsShowing) return;

        TutorialManager.Instance.IsTutorialViewing = true;
        _indexBtnController.IndexCnt = tutorialSO.tutorialList.Count;
        _indexBtnController.OnIndexChanged += StartVideo;
        _tutorialSO = tutorialSO;

        StartVideo(0);
        
        IsShowing = true;

        float startValue = 0f;
        foreach(TutorialImage image in _tutorialImageList)
        {
            float targetValue = image.targetAlphaValue;
            ShowSequence(image.image, startValue, targetValue, () =>
            {

            });
        }
    }

    public void StopTutorial(Action Callback = null)
    {
        float targetValue = 0f;
        //콜백이 여러개 들어감
        int index = 0;
        foreach(TutorialImage image in _tutorialImageList)
        {
            int tempIndex = index;
            float startValue = image.targetAlphaValue;
            ShowSequence(image.image, startValue, targetValue, () =>
            {
                if(tempIndex == 0)
                {
                    Callback?.Invoke();
                    IsShowing = false;
                    SceneControlManager.Instance.DeleteObject(this);
                }
            });
            index++;
        }

    }

    private void ShowSequence(MaskableGraphic targetImage,float startValue,float targetValue, Action Callback = null)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(targetImage.DOFade(startValue, 0f));
        seq.Append(targetImage.DOFade(targetValue, 0.85f));
        seq.AppendCallback(() =>
        {
            Callback?.Invoke();
            DOTween.Kill(this);
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
