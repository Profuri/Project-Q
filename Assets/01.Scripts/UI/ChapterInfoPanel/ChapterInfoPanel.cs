using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ChapterInfoPanel : UIComponent
{
    [SerializeField] private float _lifeTime;
    [SerializeField] private ChapterInfoData _chapterInfoData;

    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _descText;
    
    private WaitForSecondsRealtime _lifeWaitTime;

    protected override void Awake()
    {
        base.Awake();
        _lifeWaitTime = new WaitForSecondsRealtime(_lifeTime);
        _titleText = transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
        _descText = transform.Find("DescText").GetComponent<TextMeshProUGUI>();
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, () => StartCoroutine(LifeCycleRoutine()));
    }

    public void SetUp(ChapterType chapter)
    {
        var info = _chapterInfoData.GetChapterInfo(chapter);

        _titleText.text = info.chapter.ToString();
        _descText.text = info.desc;
    }

    private IEnumerator LifeCycleRoutine()
    {
        yield return _lifeWaitTime;
        Disappear();
    }
}