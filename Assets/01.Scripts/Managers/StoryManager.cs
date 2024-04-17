using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManagingSystem;
using System;

public class StoryManager : BaseManager<StoryManager>
{
    public StoryPanel CurrentPanel { get; private set; }    
    public Canvas StoryCanvas { get; private set; }

    public override void StartManager()
    {
    }

    private void Update()
    {
        CurrentPanel?.transform.LookAt(-CameraManager.Instance.MainCam.transform.position);
    }

    public void ResetMessage()
    {
        CurrentPanel = null;
    }
    public void ASDASD()
    {


    }
    public void ShowMessage(string message,Vector3 panelPos)
    {
        Action messageAction = () =>
        {
            CurrentPanel = UIManager.Instance.GenerateUI("StoryPanel") as StoryPanel;
            CurrentPanel.ResetPosition();
            //CurrentPanel.RectTrm.position = panelPos;
            CurrentPanel.AppearMessage(message);
        };

        if(CurrentPanel != null)
        {
            CurrentPanel.DisappearMessage(() =>
            {
                CurrentPanel = null;
                messageAction?.Invoke();
            });
            return;
        }
        messageAction?.Invoke();
    }
}
