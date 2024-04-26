using UnityEngine;
using Febucci.UI;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoryPanel : UIComponent, IPointerClickHandler
{
    protected TextAnimator_TMP _textAnimator;
    protected TypewriterByCharacter _typeWriter;

    protected StorySO _storySO;
    private int _currentIndex;
    private bool _isTyping;

    public RectTransform RectTrm {get; private set; }


    public bool IsActive => gameObject.activeSelf;

    protected override void Awake()
    {
        base.Awake();
        RectTrm = GetComponent<RectTransform>();
        Transform messageTrm = transform.Find("MessageText");

        _textAnimator = messageTrm.GetComponent<TextAnimator_TMP>();
        _typeWriter = messageTrm.GetComponent<TypewriterByCharacter>();
    }

    public void SettingStory(StorySO storySO,bool isTypingStory = false)
    {
        _storySO = storySO;
        _currentIndex = 0;
        this._isTyping = isTypingStory;

        string message = _storySO.contentList[_currentIndex].storyText;
        AppearMessage(message,isTypingStory);
    }

    private void NextStory()
    {
        ++_currentIndex;
        if(_currentIndex >=  _storySO.contentList.Count)
        {
            // 스토리를 전부 본 상황
            // 안봤으면 어쩔건데 XX아
            Disappear();
            return;
        }
        StoryContent content = _storySO.contentList[_currentIndex];
        string message = content.storyText;
        AppearMessage(message,_isTyping);
    }

    private void AppearMessage(string message,bool isTypingAutomatic = true)
    {
        if (!IsActive) return;
        _textAnimator.typewriterStartsAutomatically = isTypingAutomatic;
        if(isTypingAutomatic)
        {
            _typeWriter.ShowText(message);
        }
        else
        {
            _textAnimator.SetText(message);
        }
    }


    public string DisappearMessage(Action Callback = null)
    {
        if (!IsActive) return string.Empty;

        _typeWriter.StartDisappearingText();
        _typeWriter.onTextDisappeared.AddListener(() =>
        {
            Callback?.Invoke();
            SceneControlManager.Instance.DeleteObject(this);
        });
        StoryManager.Instance.ResetMessage();
        return _typeWriter.TextAnimator.textFull;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null)
        {
            NextStory();
        }
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        base.Appear(parentTrm, callback);
        CursorManager.RegisterUI(this);
    }

    public override void Disappear(Action callback = null)
    {
        base.Disappear(callback);
        CursorManager.UnRegisterUI(this);
    }
    #region TYPE_WRITER
    protected void OnEnable()  =>  _typeWriter.onMessage.AddListener(OnTypewriterMessage);
    protected void OnDisable()
    {
        _typeWriter.onMessage.RemoveListener(OnTypewriterMessage);
        StoryManager.Instance?.ResetMessage();
    }


    //원하는 string 값에다가 <?    messageName    > 이런 식으로 이벤트를 걸어줄 수 있음.
    //ex <?리그오브레전드레이븐>리그오브레전드레이븐
    private void OnTypewriterMessage(Febucci.UI.Core.Parsing.EventMarker eventMarker)
    {
        switch (eventMarker.name)
        {
            case "리그오브레전드레이븐":
                
                break;
        }
    }


    #endregion
}
