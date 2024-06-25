using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StageRoadMapUnit : UIComponent
{
    private const string DefaultBackgroundColorHex = "#FFFFFF";
    private const string ClearBackgroundColorHex = "#000000";

    private const string DefaultInnerColorHex = "#000000";
    private const string TutorialInnerColorHex = "#7637FF";
    private const string DifficultInnerColorHex = "#FFD300";

    private const float DefaultSize = 60f;
    private const float NotAccessSize = 40f;
    
    private Image _backgroundImage;
    private Image _innerImage;

    private Color _defaultBackgroundColor;
    private Color _clearBackgroundColor;

    private Color _defaultInnerColor;
    private Color _tutorialInnerColor;
    private Color _difficultInnerColor;

    [SerializeField] private float _enableTime = 0.5f;

    protected override void Awake()
    {
        base.Awake();

        _backgroundImage = GetComponent<Image>();
        _innerImage = transform.Find("InnerImage").GetComponent<Image>();
        
        ColorUtility.TryParseHtmlString(DefaultBackgroundColorHex, out _defaultBackgroundColor);
        ColorUtility.TryParseHtmlString(ClearBackgroundColorHex, out _clearBackgroundColor);

        ColorUtility.TryParseHtmlString(DefaultInnerColorHex, out _defaultInnerColor);
        ColorUtility.TryParseHtmlString(TutorialInnerColorHex, out _tutorialInnerColor);
        ColorUtility.TryParseHtmlString(DifficultInnerColorHex, out _difficultInnerColor);
    }

    public void SetUp(StageInfo info, bool cleared)
    {
        _backgroundImage.color = cleared ? _clearBackgroundColor : _defaultBackgroundColor;

        switch (info)
        {
            case StageInfo.Default:
                _innerImage.color = _defaultInnerColor;
                break;
            case StageInfo.Tutorial:
                _innerImage.color = _tutorialInnerColor;
                break;
            case StageInfo.Difficult:
                _innerImage.color = _difficultInnerColor;
                break;
        }
    }

    public void SetEnable(bool enable, float time = -1f)
    {
        if (time < 0)
        {
            time = _enableTime;
        }
        
        StartSafeCoroutine("SetEnableRoutine", SetEnableRoutine(enable, time));
    }

    private IEnumerator SetEnableRoutine(bool enable, float time)
    {
        var originSize = enable ? NotAccessSize : DefaultSize;
        var targetSize = enable ? DefaultSize : NotAccessSize;

        var current = 0f;

        ((RectTransform)transform).sizeDelta = Vector2.one * originSize; 

        while (current < time)
        {
            current += Time.deltaTime;
            var percent = current / _enableTime;
            var size = Mathf.Lerp(originSize, targetSize, percent);
            ((RectTransform)transform).sizeDelta = Vector2.one * size;
            yield return null;
        }
        
        ((RectTransform)transform).sizeDelta = Vector2.one * targetSize;
    }
}