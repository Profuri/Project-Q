using System;
using Febucci.UI;
using TMPro;
using UnityEngine;

public class MessageWindow : UIComponent
{
    private TextMeshPro _bodyText;

    private TextAnimator_TMP _textAnimator;
    private TypewriterByCharacter _typewriter;

    private StoryData _storyData;
    private int _currentIndex;

    public StoryData StoryData => _storyData;

    protected override void Awake()
    {
        base.Awake();
        _bodyText = transform.Find("Canvas/BodyText").GetComponent<TextMeshPro>();
        _bodyText.text = "";

        _textAnimator = _bodyText.GetComponent<TextAnimator_TMP>();
        _textAnimator.typewriterStartsAutomatically = true;

        _typewriter = _bodyText.GetComponent<TypewriterByCharacter>();
    }

    public override void Appear(Transform parentTrm, Action<UIComponent> callback = null)
    {
        _bodyText.text = "";

        callback += component =>
        {
            SoundManager.Instance.PlaySFX("PanelAppear", false);
        };

        base.Appear(parentTrm, callback);
    }

    public override void Disappear(Action<UIComponent> callback = null)
    {
        _typewriter.onMessage.RemoveListener(OnTypewriterMessageHandle);
        InputManager.Instance.UIInputReader.OnEnterClickEvent -= NextStory;
        SoundManager.Instance.PlaySFX("PanelAppear", false);

        InputManager.Instance.UIInputReader.OnLeftClickEventWithOutParameter -= NextStory;
        base.Disappear(callback);
    }

    public void SetData(StoryData data)
    {
        _typewriter.onMessage.AddListener(OnTypewriterMessageHandle);
        _storyData = data;
        _currentIndex = -1;
        NextStory();
        InputManager.Instance.UIInputReader.OnEnterClickEvent += NextStory;
        InputManager.Instance.UIInputReader.OnLeftClickEventWithOutParameter += NextStory;
    }
    
    private void NextStory()
    {
        if (_typewriter.isShowingText)
        {
            _typewriter.SkipTypewriter();
            return;
        }
        
        ++_currentIndex;
        if(_currentIndex >= _storyData.contentList.Count)
        {
            StoryManager.Instance.ReleaseStory();
            return;
        }
        
        var content = _storyData.contentList[_currentIndex];
        var message = content.storyText;
        
        SetText(message);
    }

    private void SetText(string message)
    {
        _typewriter.ShowText(message);
    }
    
    private void OnTypewriterMessageHandle(Febucci.UI.Core.Parsing.EventMarker eventMarker)
    {
        Core.MessageUtil.CallMessageEvent(eventMarker);
    }
}