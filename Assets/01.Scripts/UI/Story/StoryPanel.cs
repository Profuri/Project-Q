using UnityEngine;
using Febucci.UI;
using System;

public class StoryPanel : UIComponent
{
    protected TextAnimator_TMP _textAnimator;
    protected TypewriterByCharacter _typeWriter;
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

    public void AppearMessage(string message)
    {
        if (!IsActive) return;
        _textAnimator.typewriterStartsAutomatically = true;
        _typeWriter.ShowText(message);
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
        return _typeWriter.TextAnimator.textFull;
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
