using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;
using Febucci.UI;

public class StoryPanel : UIComponent
{
    protected TextAnimator_TMP _textAnimator;

    protected override void Awake()
    {
        base.Awake();
        _textAnimator = transform.Find("MessageText").GetComponent<TextAnimator_TMP>();
    }

    public void Message(string message)
    {
        _textAnimator.typewriterStartsAutomatically = true;
        //_textAnimator.
        _textAnimator.SetText(message);
    }

    [ContextMenu("Message!!!")]
    public void SetMessage()
    {
        Message("리그오브레전드레이븐 \n 리그오브레전드레이븐 \n 리그오브레전드레이븐 \n 리그오브레전드레이븐");
    }
}
