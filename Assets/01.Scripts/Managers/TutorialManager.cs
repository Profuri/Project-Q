using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [SerializeField] private TutorialSO _sampleSO;
    private void Update()
    {
       if(Input.GetKeyDown(KeyCode.T))
       {
           StartVideoClip(_sampleSO);
       }
    }

    public void StartVideoClip(TutorialSO tutorialSO)
    {
        TutorialCanvas tutorialViewer = SceneControlManager.Instance.AddObject("TutorialCanvas") as TutorialCanvas;
        tutorialViewer.ShowTutorial(tutorialSO);
    }
}
