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

    protected override void Awake()
    {
        base.Awake();
        _bodyText = transform.Find("Canvas/BodyText").GetComponent<TextMeshPro>();
        _bodyText.text = "";

        _textAnimator = _bodyText.GetComponent<TextAnimator_TMP>();
        _textAnimator.typewriterStartsAutomatically = true;

        _typewriter = _bodyText.GetComponent<TypewriterByCharacter>();
    }

    public override void Appear(Transform parentTrm, Action callback = null)
    {
        _typewriter.onMessage.AddListener(OnTypewriterMessageHandle);
        InputManager.Instance.UIInputReader.OnEnterClickEvent += NextStory;
        base.Appear(parentTrm, callback);
    }

    public override void Disappear(Action callback = null)
    {
        _typewriter.onMessage.RemoveListener(OnTypewriterMessageHandle);
        InputManager.Instance.UIInputReader.OnEnterClickEvent -= NextStory;
        base.Disappear(callback);
    }

    public void SetData(StoryData data)
    {
        _storyData = data;
        _currentIndex = -1;
        NextStory();
    }
    
    private void NextStory()
    {
        if (_typewriter.isShowingText)
        {
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
        switch (eventMarker.name)
        {
            case "someevent":
                Debug.Log(eventMarker.parameters[0]);
                break;
        }
    }
}