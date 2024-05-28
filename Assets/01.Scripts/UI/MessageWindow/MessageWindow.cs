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
        _bodyText.text = "";
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
            case "camDampingChange":
            {
                var xDamping = Convert.ToSingle(eventMarker.parameters[0]);
                var yDamping = Convert.ToSingle(eventMarker.parameters[1]);
                var zDamping = Convert.ToSingle(eventMarker.parameters[2]);
                CameraManager.Instance.ActiveVCam.SetDamping(new Vector3(xDamping, yDamping, zDamping));
                break;
            }
            case "camFollowTargetChange":
            {
                var targetName = eventMarker.parameters[0];
                CameraManager.Instance.ActiveVCam.SetFollowTarget(GameObject.Find(targetName).transform);
                break;
            }
            case "camOffsetChange":
            {
                var offsetX = Convert.ToSingle(eventMarker.parameters[0]);
                var offsetY = Convert.ToSingle(eventMarker.parameters[1]);
                var offsetZ = Convert.ToSingle(eventMarker.parameters[2]);
                var offset = new Vector3(offsetX, offsetY, offsetZ);
                CameraManager.Instance.ActiveVCam.SetOffset(offset);
                break;
            }
            case "camSizeChange":
            {
                var targetSize = Convert.ToSingle(eventMarker.parameters[0]);
                var time = Convert.ToSingle(eventMarker.parameters[1]);
                CameraManager.Instance.ActiveVCam.Zoom(targetSize, time);
                break;
            }
        }
    }
}